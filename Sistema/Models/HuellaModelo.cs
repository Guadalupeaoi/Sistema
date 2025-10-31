namespace Sistema.Models
{
    public class HuellaModelo
    {
        public int IdHuella { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public int DedoIndex { get; set; }
        public string? TemplateHuella { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
