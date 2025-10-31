using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Sistema.Models
{
    public class observacionesModel
    {
        public int id_observaciones { get; set; }

        public int codigo {  get; set; }

        public string? descripcion { get; set; } = string.Empty;

    }
}
