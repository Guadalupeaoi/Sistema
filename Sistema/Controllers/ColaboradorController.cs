using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using Sistema.Models;
using Sistema.Funciones;
using Sistema.SistemaBL.Propiedades;
using Sistema.Entidad;

namespace Sistema.Controllers
{

    [Authorize]
    public class ColaboradorController : Controller
    {
        public IActionResult Index(string MsgToServer = "")
        {
            try
            {
                ColaboradorModel model = new ColaboradorModel();

                List<SelectListItem> comboColaborador = new List<SelectListItem>();
                comboColaborador.Add(new SelectListItem { Value = "", Text = "Selecciona el colaborador" });
                List<SelectListItem> comboAsistencia = new List<SelectListItem>();
                comboAsistencia.Add(new SelectListItem { Value = "", Text = "Seleccion la asistencia" });

                model.num_empleado = new Combos().ComboColaborador(false);
                model.nombres = comboColaborador;
                model.apeliidos = comboColaborador;
                model.Numempleado = comboAsistencia;

                if (Request.Query.ContainsKey("MsgToServer"))
                {
                    ViewBag.MsgServer = MsgToServer;
                }

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "ManagementError");
            }
        }

        [HttpGet]
        public JsonResult GetConsultaNombre(string word, int page, int rows, string searching, int Numempleado, string nombres, string apellidos)
        {
            try
            {
                List<ColaboradorModel> listaColab = new List<ColaboradorModel>();
                colaboradorBL colabObj = new colaboradorBL();

                if (searching is null)
                    searching = string.Empty;

                List<colaboradorEl> colabList = colabObj.ConsultaNombre(searching, Numempleado, nombres, apellidos, 0);

                foreach (colaboradorEl itemColab in colabList.Skip((page - 1) * rows).Take(rows).ToList())

                {
                    ColaboradorModel itemModel = new ColaboradorModel();
                    itemModel.num_empleado = itemColab.Num_empleado;
                    itemModel.nombre = itemColab.nombre;
                    itemModel.apellido = itemColab.apellido;
                    itemModel.area = itemColab.area;

                    listaColab.Add(itemModel);
                }

                int totalRecords = colabList.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = listaColab
                };

                return Json(jsonData);
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Edit(int NumEmpleado) {

            try
            {
                colaboradorEl colaboradorEl
            }
        }
    }
}
