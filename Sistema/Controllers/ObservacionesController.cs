using Microsoft.AspNetCore.Mvc;
using Sistema.SistemaBL.Propiedades;

namespace Sistema.Controllers
{
    public class ObservacionesController : Controller
    {
        private readonly observacionesBL ObservacionesBL;

        public ObservacionesController(IConfiguration configuration)
        {
            ObservacionesBL = new observacionesBL(configuration);
        }

        public IActionResult DescripcionCodigo(int codigo)
        { 
            string? descripcion = ObservacionesBL.ObtenerDescripcion(codigo);
            if (descripcion == null)
                descripcion = "Sin descripcion";

            ViewBag.Codigo = codigo;
            ViewBag.Descripcion = descripcion;
            return View();
        }

    }
}
