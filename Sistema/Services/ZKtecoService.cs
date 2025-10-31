using Sistema.AppStart;
using Sistema.Models;
using Sistema.SistemaDL.Propiedades;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using zkemkeeper;

namespace Sistema.Services
{
    public class ZKtecoService
    {
        private CZKEM device;
        private readonly ApplicationDbContext _context;
        private readonly CZKEMClass dispositivo = new CZKEMClass();
       

        public ZKtecoService(ApplicationDbContext context)
        {
            _context = context;
            device = new CZKEM();
        }

        public bool Conectar(string ip, int puerto = 4370)
        {
            bool conectado = dispositivo.Connect_Net(ip, puerto);
            if (!conectado)
            {
                int error = 0;
                dispositivo.GetLastError(ref error);
                Console.WriteLine($"❌ No se pudo conectar al reloj. Código de error: {error}");
            }
            else
            {
                Console.WriteLine("✅ Conexión establecida correctamente con el reloj ZKTeco.");
            }

            return conectado;
        }
        public List<asistenciaModel> LeerRegistros(string ip)
        {
            var registros = new List<asistenciaModel>();

            if (!Conectar(ip))
            {
                Console.WriteLine("❌ No se pudo conectar con el reloj.");
                return registros;
            }

            Console.WriteLine("✅ Conectado al reloj. Obteniendo mapa de usuarios...");
            var mapa = ObtenerUsuariosDesdeReloj(ip)
                .ToDictionary(u => u.EnrollNumber?.Trim(), u => u.Name ?? $"Empleado {u.EnrollNumber}");

            int[] machineNumbersToTry = new[] { 1, 0 };

            foreach (var machine in machineNumbersToTry)
            {
                Console.WriteLine($"Probando machineNumber = {machine} ...");
                dispositivo.ReadGeneralLogData(machine);

                string dwEnrollNumber;
                int dwVerifyMode, dwInOutMode, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond;
                int dwWorkCode = 0;

                try
                {
                    while (dispositivo.SSR_GetGeneralLogData(machine, out dwEnrollNumber,
                            out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay,
                            out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                    {
                        if (string.IsNullOrWhiteSpace(dwEnrollNumber))
                            continue;

                        DateTime fechaHora;
                        try
                        {
                            fechaHora = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
                        }
                        catch
                        {
                            continue; // ignorar fechas inválidas
                        }

                        mapa.TryGetValue(dwEnrollNumber.Trim(), out string nombre);
                        nombre ??= $"Empleado {dwEnrollNumber.TrimStart('0')}";

                        registros.Add(new asistenciaModel
                        {
                            num_empleado_reloj = dwEnrollNumber.TrimStart('0'),
                            colaborador = new ColaboradorModel { nombre = nombre },
                            fecha = fechaHora.Date,
                            hora_entrada = dwInOutMode == 0 ? fechaHora : (DateTime?)null,
                            hora_salida = dwInOutMode == 1 ? fechaHora : (DateTime?)null,
                            estado = "Presente"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error leyendo logs con machine={machine}: {ex.Message}");
                }
            }

            dispositivo.Disconnect();

            // 🔹 Agrupar por empleado y fecha combinando entrada y salida
            var registrosLimpios = registros
                .GroupBy(r => new { r.num_empleado_reloj, r.fecha })
                .Select(g =>
                {
                    var entrada = g.Where(x => x.hora_entrada.HasValue).OrderBy(x => x.hora_entrada).FirstOrDefault();
                    var salida = g.Where(x => x.hora_salida.HasValue).OrderByDescending(x => x.hora_salida).FirstOrDefault();

                    return new asistenciaModel
                    {
                        num_empleado_reloj = g.Key.num_empleado_reloj,
                        colaborador = entrada?.colaborador ?? salida?.colaborador,
                        fecha = g.Key.fecha,
                        hora_entrada = entrada?.hora_entrada,
                        hora_salida = salida?.hora_salida ?? entrada?.hora_entrada,
                        estado = "Presente"
                    };
                })
                .OrderBy(x => x.fecha)
                .ThenBy(x => x.num_empleado_reloj)
                .ToList();

            Console.WriteLine($"✅ Total registros originales: {registros.Count}");
            Console.WriteLine($"✅ Total registros limpios: {registrosLimpios.Count}");

            return registrosLimpios;
        }


        // Updated the line causing the error by modifying the `ObtenerNombreUsuario` method to return a `ColaboradorModel` instead of a `string`.
        // This ensures compatibility with the `colaborador` property of type `ColaboradorModel`.

        private Dictionary<string, string> ObtenerMapaUsuarios(string ip)
        {
            var mapa = new Dictionary<string, string>();
            string enrollNumber = "", name = "", password = "";
            int privilege = 0;
            bool enabled = false;

            if (!dispositivo.Connect_Net(ip, 4370))
            {
                Console.WriteLine("❌ No se pudo conectar al reloj para obtener usuarios.");
                return mapa;
            }

            dispositivo.ReadAllUserID(1);

            while (dispositivo.SSR_GetAllUserInfo(1, out enrollNumber, out name, out password, out privilege, out enabled))
            {
                if (!string.IsNullOrWhiteSpace(enrollNumber))
                {
                    mapa[enrollNumber] = string.IsNullOrWhiteSpace(name) ? $"Empleado {enrollNumber}" : name;
                    Console.WriteLine($"👤 Usuario mapeado: {enrollNumber} → {name}");
                }
            }

            dispositivo.Disconnect();
            return mapa;
        }

        public List<(string EnrollNumber, string Name)> ObtenerUsuariosDesdeReloj(string ip)
        {
            var listaUsuarios = new List<(string, string)>();
            CZKEMClass device = new CZKEMClass();

            if (!device.Connect_Net(ip, 4370))
            {
                Console.WriteLine("❌ No se pudo conectar al reloj checador.");
                return listaUsuarios;
            }

            string enrollNumber = "";
            string name = "";
            string password = "";
            int privilege = 0;
            bool enabled = false;

            if (device.ReadAllUserID(1))
            {
                while (device.SSR_GetAllUserInfo(1, out enrollNumber, out name, out password, out privilege, out enabled))
                {
                    listaUsuarios.Add((enrollNumber, name));
                    Console.WriteLine($" Usuario leído: {enrollNumber} - {name}");
                }
            }

            device.Disconnect();
            return listaUsuarios;
        }


      
    
        public void Desconectar()
        {
            dispositivo.Disconnect();
        }

       
    }

}
