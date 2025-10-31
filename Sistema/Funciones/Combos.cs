using Sistema.Entidad;
using System.Reflection.Metadata.Ecma335;
using Sistema.SistemaBL.Propiedades;
using System.Web.Mvc;

namespace Sistema.Funciones
{
    public class Combos
    {
        public List<SelectListItem> ComboColaborador(string Nombres, string Apellidos, bool EsRequerido = false)
        {
            List<SelectListItem> comboColaborador = new List<SelectListItem>();

            if (EsRequerido)
                comboColaborador.Add(new SelectListItem { Value = "", Text = "Seleccione un colaborador" });
            else
                comboColaborador.Add(new SelectListItem { Value = "0", Text = "Seleccione un colaborador" });
            List<colaboradorEl> colaList = new colaboradorBL().ConsultaNombre(Nombres, 0, 0, Apellidos, Apellidos);

            foreach (colaboradorEl itemCola in colaList)
            {
                SelectListItem itemList = new SelectListItem();
                itemList.Value = itemCola.Num_empleado.ToString();
                itemList.Text = itemCola.nombre;
                itemList.Text = itemCola.apellido;

                comboColaborador.Add(itemList);

            }

            return comboColaborador;

        }

        public List<SelectListItem> ComboAsistencia(int id_asistencia, bool EsRequerido = false)
        {
            List<SelectListItem> comboAsistencia = new List<SelectListItem>();

            if (EsRequerido)
                comboAsistencia.Add(new SelectListItem { Value = "", Text = "Seleccione la asistencia" });
            else
                comboAsistencia.Add(new SelectListItem { Value = "0", Text = "Seleccione la asistencia" });

            List<asistenciaEL> asisList = new asistenciaBL().ConsultaAsistencia("", id_asistencia);

            foreach (asistenciaEL itemAsistencia in asisList)
            {
                SelectListItem itemList = new SelectListItem();
                itemList.Value = itemAsistencia.Id_asistencia.ToString();
                itemList.Value = itemAsistencia.colaborador;
                itemList.Text = itemAsistencia.Estado;
                itemList.Value = itemAsistencia.Hora_entrada.ToString();
                itemList.Value = itemAsistencia.Hora_salida.ToString();
                itemList.Value = itemAsistencia.Fecha.ToString();
                itemList.Value = itemAsistencia.Dias_labores.ToString();

                comboAsistencia.Add(itemList);
            }
            return comboAsistencia;
        }

        public SelectList ComboDias(DateTime Fecha, DateTime Dias_laborales, bool EsRequerido = false)
        {
            var items = new List<SelectListItem>();

            if (EsRequerido)
                items.Add(new SelectListItem { Value = "", Text = "Selecciona el día" });
            else
                items.Add(new SelectListItem { Value = "0", Text = "Selecciona el día" });

            List<asistenciaEL> asisList = new asistenciaBL().ConsultaDias("",Fecha, Dias_laborales);

            foreach (var itemDia in asisList)
            {
                items.Add(new SelectListItem
                {
                    Value = itemDia.Fecha.ToString("yyyy-MM-dd"),
                    Text = itemDia.Dias_labores.ToString()
                });
            }

            return new SelectList(items, "Value", "Text");
        }

        public List<SelectListItem> ComboSemana(int id_nomina, bool EsRequerido = false)
        {
            List<SelectListItem> comboSemana = new List<SelectListItem>();

            if (EsRequerido)
                comboSemana.Add(new SelectListItem { Value = "", Text = "Seleccione una semana" });
            else
                comboSemana.Add(new SelectListItem { Value = "", Text = "Selecciona una semana" });

            List<nominaSemanalEL> listaNomi = new nomina_semanalBL().ConsultaSemana(id_nomina);

            foreach (nominaSemanalEL itemNomi in listaNomi)
            {
                SelectListItem itemList = new SelectListItem();
                itemList.Value = itemNomi.id_nomina.ToString();
                itemList.Text = itemNomi.periodo_inicial.ToString();
                itemList.Text = itemNomi.periodo_final.ToString();

                comboSemana.Add (itemList);
            }

            return comboSemana;

        }

        public List<SelectListItem> ComboObservaciones(int codigo, bool EsRequerido = false)
        {
            List<SelectListItem> comboObservaciones = new List<SelectListItem>();

            if (EsRequerido)
                comboObservaciones.Add(new SelectListItem { Value = "", Text = "Selecciona las observaciones" });
            else
                comboObservaciones.Add(new SelectListItem { Value = "", Text = "Selecciona las observaciones" });

            List<observacionesEL> listaObser = new observacionesBL().ConsultaObservaciones(codigo);

            foreach (observacionesEL itemObser in listaObser )
            {
                SelectListItem itemList = new SelectListItem();
                itemList.Value = itemObser.codigo.ToString();
                itemList.Text = itemObser.descripcion;

                comboObservaciones.Add (itemList);
            }

            return comboObservaciones;
        }

        internal int ComboAsistencia(bool v)
        {
            throw new NotImplementedException();
        }

      
        
    }
}
