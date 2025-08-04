using Microsoft.AspNetCore.Mvc;
using Sistema.Models;
using Sistema.Funciones;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Sistema.SistemaBL.Propiedades;
using Sistema.Entidad;

namespace Sistema.Controllers
{
    public class asistenciaController : Controller
    {
        public ActionResult Index(string MsgToServer = "")
/// Editar la entidad asistencia para agregar el colaborador para el funcionamiento de la consulta
        {
            try
            {
                asistenciaModel model = new asistenciaModel();
              

                List<SelectListItem> comboAsistencia = new List<SelectListItem>();
                comboAsistencia.Add(new SelectListItem { Value = "", Text = "Selecciona la asistencia" });
                List<SelectListItem> comboColaborador = new List<SelectListItem>();
                comboColaborador.Add(new SelectListItem { Value = "", Text = "Selecciona el colaborador" });

                model.id_asistencia = new Combos().ComboAsistencia(false);
                model.fecha = DateTime.Now;
                model.hora_entrada = TimeSpan.MinValue;
                model.hora_salida = TimeSpan.MinValue;
                model.dias_laborales = DateTime.Today;
                model.id_asistencia = 3;



                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "ManagemtError");

            }
        }

        [HttpGet]
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
        public JsonResult GetConsultaDias(string word, int page, int rows, string searchString, DateTime Fecha, DateTime Dias_laborales )
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
    }
}   



