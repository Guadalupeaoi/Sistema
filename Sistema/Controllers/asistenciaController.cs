using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema.AppStart;
using System.ComponentModel.DataAnnotations;
using X.PagedList;
using Sistema.Entidad;
using Sistema.Funciones;
using Sistema.Models;
using Sistema.Services;
using Sistema.SistemaBL.Propiedades;
using Sistema.SistemaDL.Propiedades;
using SixLabors.Fonts;
using System.Drawing;
using System.Text;
using zkemkeeper;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace Sistema.Controllers
{
    public class asistenciaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ZKtecoService _zkService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public asistenciaController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _zkService = new ZKtecoService(_context);
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(
     int page = 1,
     int pageSize = 10,
     string nombre = null,
     DateTime? fecha = null,
     int? semana = null,
     int? año = null)
        {
            var query = _context.Asistencias
                .Include(a => a.colaborador)
                .AsQueryable();

            // 🔍 Filtro por nombre
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                string nombreLower = nombre.ToLower();
                query = query.Where(a => a.colaborador != null && a.colaborador.nombre.ToLower().Contains(nombreLower));
            }

            // 📅 Filtro por fecha exacta
            if (fecha.HasValue)
            {
                var fechaFiltro = fecha.Value.Date;
                query = query.Where(a => a.fecha.Date == fechaFiltro);
            }

            // 🗓️ Filtro por semana (ISO 8601)
            if (semana.HasValue && año.HasValue)
            {
                // Calcula el lunes de la semana ISO
                DateTime inicioSemana = FirstDateOfWeekISO8601(año.Value, semana.Value);
                DateTime finSemana = inicioSemana.AddDays(6);

                query = query.Where(a => a.fecha.Date >= inicioSemana && a.fecha.Date <= finSemana);
            }

            // 🔽 Orden y paginación
            var asistenciasFiltradas = query
                .OrderByDescending(a => a.fecha)
                .ThenBy(a => a.hora_entrada)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalRegistros = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // 🧭 Pasar datos a la vista
            ViewBag.Nombre = nombre;
            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");
            ViewBag.Semana = semana;
            ViewBag.Año = año ?? DateTime.Now.Year;
            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;

            return View(asistenciasFiltradas);
        }

        // 📘 Método auxiliar para calcular el primer día (lunes) de una semana ISO
        private static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            int firstWeek = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                firstThursday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }


        // Conectar al reloj por IP
        public IActionResult ConectarReloj(string ip)
        {
            bool conectado = _zkService.Conectar(ip);

            return Json(new
            {
                success = conectado,
                message = conectado ? "Conexión exitosa al reloj." : "No se pudo conectar al reloj."
            });
        }

        // Sincronizar registros desde ZKTeco y guardarlos en la base de datos
        public IActionResult Sincronizar(string ip = "192.168.11.201")
        {
            var registros = _zkService.LeerRegistros(ip);

            if (registros == null || !registros.Any())
            {
                ViewBag.TotalPaginas = 1;
                ViewBag.PaginaActual = 1;
                return View("Index", new List<asistenciaModel>());
            }

            // 🔹 Consolidar registros por empleado y fecha
            var agrupados = registros
      .AsEnumerable() // Traer a memoria primero
      .GroupBy(r => new { r.num_empleado_reloj, Fecha = r.fecha.Date })
      .Select(g =>
      {
          var primeraMarca = g.OrderBy(x => x.hora_entrada).First();
          var ultimaMarca = g.OrderByDescending(x => x.hora_entrada).First();

          return new
          {
              NumReloj = g.Key.num_empleado_reloj.TrimStart('0'),
              Fecha = g.Key.Fecha,
              HoraEntrada = primeraMarca.hora_entrada,
              HoraSalida = ultimaMarca.hora_entrada
          };
      })
      .ToList();
            foreach (var r in agrupados)
            {
                // Evitar duplicados
                bool existe = _context.Asistencias
                    .AsEnumerable() // Trae registros a memoria
                    .Any(a => a.num_empleado_reloj.TrimStart('0') == r.NumReloj &&
                              a.fecha.Date == r.Fecha);

                if (existe) continue;

                var colaborador = _context.Colaboradores
                    .FirstOrDefault(c => c.num_empleado.ToString() == r.NumReloj);

                if (colaborador == null) continue;

                var nuevo = new asistenciaModel
                {
                    num_empleado = colaborador.num_empleado,
                    colaborador = colaborador,
                    num_empleado_reloj = r.NumReloj,
                    fecha = r.Fecha,
                    hora_entrada = r.HoraEntrada,
                    hora_salida = r.HoraSalida,
                    estado = "Presente",
                    dias_laborales = DateTime.Now
                };

                _context.Asistencias.Add(nuevo);
            }

            _context.SaveChanges();


            // 🔹 Traer asistencias actualizadas sin duplicar
            var asistencias = _context.Asistencias
                .Include(a => a.colaborador)
                .AsEnumerable()
                .GroupBy(a => new { a.num_empleado_reloj, Fecha = a.fecha.Date })
                .Select(g =>
                {
                    var entrada = g.OrderBy(x => x.hora_entrada).First();
                    var salida = g.OrderByDescending(x => x.hora_entrada).First();

                    return new asistenciaModel
                    {
                        num_empleado_reloj = g.Key.num_empleado_reloj,
                        colaborador = entrada.colaborador,
                        fecha = g.Key.Fecha,
                        hora_entrada = entrada.hora_entrada,
                        hora_salida = salida.hora_salida ?? salida.hora_entrada,
                        estado = "Presente"
                    };
                })
                .OrderByDescending(a => a.fecha)
                .ThenBy(a => a.num_empleado_reloj)
                .ToList();

            ViewBag.TotalPaginas = 1;
            ViewBag.PaginaActual = 1;



            return View("Index", asistencias);
        }

        // Guardar registros manualmente (opcional)
        [HttpPost]
        public IActionResult GuardarRegistros(List<asistenciaModel> registros)
        {
            if (registros != null && registros.Any())
            {
                _context.Asistencias.AddRange(registros);
                _context.SaveChanges();
                ViewBag.Mensajes = new List<string> { "✅ Registros guardados correctamente." };
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var rol = _httpContextAccessor.HttpContext.Session.GetString("Rol");
            var asistencia = _context.Asistencias
          .Include(a => a.colaborador)
          .FirstOrDefault(a => a.id_asistencia == id);

            if (asistencia == null)
                return NotFound();


            // Bloquear si no es Dirección General
            if (rol != "diradmin")
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }


            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asistencia/Edit/5
        [HttpPost]
        [Authorize(Roles = "diradmin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, asistenciaModel model)
        {
            // 🔹 1. Validar rol del usuario desde la sesión
            var rol = _httpContextAccessor.HttpContext.Session.GetString("Rol");
            if (rol != "diradmin")
            {
                return Unauthorized(); // Solo Dirección General puede editar
            }

            // 🔹 2. Validar que el ID coincida
            if (id != model.id_asistencia)
            {
                return NotFound();
            }

            // 🔹 3. Validar que el modelo sea correcto
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Verifica los datos ingresados.";
                return View(model);
            }

            // 🔹 4. Buscar el registro existente
            var asistenciaExistente = _context.Asistencias.FirstOrDefault(a => a.id_asistencia == id);
            if (asistenciaExistente == null)
            {
                return NotFound();
            }

            try
            {
                // 🔹 5. Actualizar solo los campos editables
                asistenciaExistente.fecha = model.fecha;
                asistenciaExistente.hora_entrada = model.hora_entrada;
                asistenciaExistente.hora_salida = model.hora_salida;
                asistenciaExistente.estado = model.estado;

                // 🔹 6. Guardar los cambios
                _context.SaveChanges();

                // 🔹 7. Mensaje de confirmación y redirección
                TempData["Mensaje"] = "✅ Los cambios en la asistencia se guardaron correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // 🔹 8. Manejo de error si otro proceso modificó el mismo registro
                if (!_context.Asistencias.Any(a => a.id_asistencia == id))
                    return NotFound();

                throw; // Si fue otro error, relanzar la excepción
            }
        }

    }
}
           /*
        catch (DbUpdateException ex)
            {
                // Esto mostrará la causa SQL exacta
                return BadRequest($"Error al guardar cambios: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error general: {ex.Message}");
            }
        }


    }
}
    





        /* [HttpGet]
         public JsonResult GetConsultaAsistencia(string word, int page, int rows, string searchString, int Id_asistencia, DateTime Fecha, TimeSpan Hora_entrada, TimeSpan Hora_salida, string Estado, DateTime Dias_Laborales)
         {
             try
             {
                 List<asistenciaModel> listaAsis = new List<asistenciaModel>();
                 asistenciaBL asisObj = new asistenciaBL();

                 if (searchString is null)
                     searchString = string.Empty;

                 if (Estado is null)
                     Estado = string.Empty;

                 List<asistenciaEL> asisList = asisObj.ConsultaAsistencia(searchString, Id_asistencia);

                 foreach (asistenciaEL itemAsis in asisList.Skip((page - 1) * rows).Take(rows).ToList())
                 {

                     asistenciaModel itemModel = new asistenciaModel();
                     DateTime fechaLlegada = new DateTime(2025, 01, 01);

                     itemModel.id_asistencia = itemAsis.Id_asistencia;
                     itemModel.colaborador = itemAsis.colaborador;
                     itemModel.fecha = itemAsis.Fecha;
                     itemModel.hora_entrada = itemAsis.Hora_entrada;
                     itemModel.hora_salida = itemAsis.Hora_salida;
                     itemModel.estado = itemAsis.Estado;
                     itemModel.dias_laborales = itemAsis.Dias_labores;

                     listaAsis.Add(itemModel);


                 }

                 int totalRecords = asisList.Count();
                 var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                 var jsonData = new
                 {
                     total = totalPages,
                     page,
                     records = totalRecords,
                     rows = listaAsis
                 };

                 return Json(jsonData);
             }
             catch (Exception ex)
             {
                 return null;

             }

         }

         [HttpGet]
         public JsonResult GetConsultaDias(string word, int page, int rows, string searchString, DateTime Fecha, DateTime Dias_laborales)
         {
             try
             {
                 List<asistenciaModel> listaDia = new List<asistenciaModel>();
                 asistenciaBL diaObj = new asistenciaBL();

                 List<asistenciaEL> diaList = diaObj.ConsultaDias(searchString, Fecha, Dias_laborales);

                 foreach (asistenciaEL itemDia in diaList.Skip((page - 1) * rows).Take(rows).ToList())
                 {
                     asistenciaModel itemModel = new asistenciaModel();
                     itemModel.fecha = itemDia.Fecha;
                     itemModel.dias_laborales = Dias_laborales;

                     listaDia.Add(itemModel);
                 }

                 int totalRecords = listaDia.Count;
                 var totalPages = (int)Math.Ceiling((float)totalRecords / rows);

                 var jsonData = new
                 {
                     totalRecords = totalPages,
                     page,
                     records = totalRecords,
                     rows = listaDia
                 };


                 return Json(jsonData);
             }
             catch (Exception ex)
             {
                 return null;
             }


         }

         [HttpGet]
         public ActionResult ExportarExcel(int id_asistencia, int NumEmpleado, string? nombre, string? apellido, DateTime? Fecha, TimeSpan Hora_entrada, TimeSpan Hora_salida, DateTime? Dias_Laborales, int id_observaciones)
         {
             try
             {
                 string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes");
                 string reportName = "\\ReporteAsistencia" + DateTime.Now.ToString("ddMMyy HHmmss") + ".xls";
                 string contraseña = "PCLATEZDIRECCION";

                 asistenciaBL asisObj = new asistenciaBL();
                 DateTime fecha = new DateTime(2023, 01, 01);

                 if (Dias_Laborales != null)
                     Fecha = Dias_Laborales.Value;

                 List<asistenciaEL> asisList = asisObj.ConsultaAsistencia(id_asistencia, NumEmpleado, nombre, apellido, Fecha, Hora_entrada, Hora_salida, Dias_Laborales, id_observaciones, 0);

                 Excel.Application excelApp = new Excel.Application();

                 Excel.Workbook excelWorkBook = excelApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATExcel4MacroSheet);

                 excelWorkBook.Password = "";
                 excelWorkBook.WritePassword = "PCLATEZDIRECCION";

                 excelWorkBook.SaveAs(reportPath + reportName,
                 Excel.XlFileFormat.xlOpenXMLWorkbook,Type.Missing,"PCLATEZDIRECCION",
                 false,      
                 false,      
                 Excel.XlSaveAsAccessMode.xlExclusive,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                 Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                 excelWorkSheet.Name = "VTA_INS_" + DateTime.Now.ToString("ddMMyy");


                 excelWorkSheet.Range["A1", "A7"].Merge();
                 excelWorkSheet.Cells[1, 4] = "CONTROL DE ASISTENCIAS PERSONAL DE OFICINAS";
                 excelWorkSheet.Range["B1", "H2"].Merge();
                 excelWorkSheet.Range["B1", "H2"].Font.Name = "Times New Roman";
                 excelWorkSheet.Range["B1", "H7"].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(0, 128, 0));

                 string[,] sHeads = new string[1, 8];

                 excelWorkSheet.Cells[2, 1] = "No.Empleado";
                 excelWorkSheet.Cells[2, 2] = "Nombre";
                 excelWorkSheet.Cells[2, 3] = "Sabado";
                 excelWorkSheet.Cells[2, 4] = "Lunes";
                 excelWorkSheet.Cells[2, 5] = "Martes";
                 excelWorkSheet.Cells[2, 6] = "Miercoles";
                 excelWorkSheet.Cells[2, 7] = "Jueves";
                 excelWorkSheet.Cells[2, 8] = "Viernes";
                 excelWorkSheet.Cells[2, 9] = "Estado";
                 excelWorkSheet.Cells[2, 10] = "Fecha";


                 int rowCount = 8;
                 int columnCount = 0;

                 foreach (asistenciaEL itemAsis in asisList)
                 {
                     string[,] sValues = new string[1, 8];


                     columnCount = 0;
                     sValues[0, columnCount] = itemAsis.colaborador;
                     columnCount++;
                     sValues[1, columnCount] = itemAsis.Hora_entrada.ToString(@"hh\:mm");
                     columnCount++;
                     sValues[2, columnCount] = itemAsis.Hora_salida.ToString(@"hh\:mm");
                     columnCount++;
                     sValues[3, columnCount] = itemAsis.Dias_labores.ToString("dd/MM/yyy") == "01/01/2023" ? String.Empty : itemAsis.Dias_labores.ToString("dd/MM/yyyy");
                     columnCount++;
                     sValues[4, columnCount] = itemAsis.Estado;
                     columnCount++;
                     sValues[5, columnCount] = itemAsis.Fecha.ToString("dd/MM/yyy") == "01/01/2023" ? String.Empty : itemAsis.Dias_labores.ToString("dd/MM/yyyy");


                     excelWorkSheet.get_Range("A" + rowCount.ToString(), "H" + rowCount.ToString()).Value2 = sValues;
                     rowCount++;
                 }
                 rowCount--;

                 //Estilos en cuerpo de Reporte
                 excelWorkSheet.Range["B9", "B" + rowCount.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                 excelWorkSheet.Range["B9", "B" + rowCount.ToString()].Borders[Excel.XlBordersIndex.xlEdgeLeft].Color = ColorTranslator.ToOle(Color.Black);
                 excelWorkSheet.Range["C9", "C" + rowCount.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                 excelWorkSheet.Range["C9", "C" + rowCount.ToString()].Borders[Excel.XlBordersIndex.xlEdgeLeft].Color = ColorTranslator.ToOle(Color.Black);
                 excelWorkSheet.Range["C9", "G" + rowCount.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                 excelWorkSheet.Range["H9", "H" + rowCount.ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                 excelWorkSheet.Range["A9", "H" + rowCount.ToString()].Font.Size = 10;
                 excelWorkSheet.Range["A8", "H" + rowCount.ToString()].Font.Name = "Arial";
                 excelWorkSheet.Range["A9", "H" + rowCount.ToString()].Interior.Color = ColorTranslator.ToOle(Color.FromArgb(245, 255, 250));
                 excelWorkSheet.Range["A8", "H8"].Font.FontStyle = FontStyle.Bold;//Negrita de encabezado                
                 excelWorkSheet.Range["B1", "H2"].Font.Size = 18;
                 excelWorkSheet.Range["B1", "H2"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                 excelWorkSheet.Range["B1", "H2"].Font.FontStyle = FontStyle.Bold;//Negrita de encabezado

                 if (!System.IO.Directory.Exists(reportPath))
                     System.IO.Directory.CreateDirectory(reportPath);

                 excelWorkBook.SaveAs(reportPath + reportName,
                 Excel.XlFileFormat.xlExcel12, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                 excelWorkBook.Close();
                 excelApp.Quit();

                 byte[] fileBytes = System.IO.File.ReadAllBytes(reportPath + reportName);
                 return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, reportName);

             }
             catch (Exception ex)
             {
                 byte[] fileBytes = Encoding.ASCII.GetBytes("Mensaje::" + ex.Message + "::::::::StackTrace:" + ex.StackTrace);
                 return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LogError_" + DateTime.Now.ToString("ddMMyy HHmmss") + ".txt");
             }
         }

     }*/





