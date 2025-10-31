using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Sistema.AppStart;
using ClosedXML.Excel;

namespace Sistema.Controllers
{
    public class ReportesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerarReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            var registros = await _context.Asistencias
                .Include(a => a.colaborador)
                .Where(a => a.fecha >= fechaInicio && a.fecha <= fechaFin)
                .OrderBy(a => a.num_empleado_reloj)
                .ThenBy(a => a.fecha)
                .ToListAsync();

            return View("ReporteSemanal", registros);
        }

        [HttpPost]
        public async Task<IActionResult> DescargarExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            var registros = await _context.Asistencias
                .Include(a => a.colaborador)
                .Where(a => a.fecha >= fechaInicio && a.fecha <= fechaFin)
                .OrderBy(a => a.num_empleado_reloj)
                .ThenBy(a => a.fecha)
                .ToListAsync();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Entradas y Salidas");

                // ==============================
                // 🔹 ENCABEZADO
                // ==============================
                ws.Row(1).Height = 60;
                ws.Row(2).Height = 25;
                ws.Row(3).Height = 25;

                string logo1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "img", "Logo.png");
                string logo2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "img", "Calidad.png");

                if (System.IO.File.Exists(logo1))
                    ws.AddPicture(logo1).MoveTo(ws.Cell("A1")).WithSize(130, 55);

                if (System.IO.File.Exists(logo2))
                    ws.AddPicture(logo2).MoveTo(ws.Cell("B1")).WithSize(130, 55);

                ws.Range("C1:L1").Merge().Value = "ENTRADAS Y SALIDAS";
                ws.Range("C1:L1").Style.Font.Bold = true;
                ws.Range("C1:L1").Style.Font.FontSize = 14;
                ws.Range("C1:L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("C1:L1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                string rangoFechas = $"DEL {fechaInicio:dd 'DE' MMMM} AL {fechaFin:dd 'DE' MMMM 'DEL' yyyy}".ToUpper();
                ws.Range("C2:L2").Merge().Value =
                    $"CONTROL DE ASISTENCIA PERSONAL DE OFICINAS CORRESPONDIENTE AL PERIODO {rangoFechas}";
                ws.Range("C2:L2").Style.Font.Bold = true;
                ws.Range("C2:L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("C2:L2").Style.Alignment.WrapText = true;

                // ==============================
                // 🔹 ENCABEZADOS DE COLUMNA
                // ==============================
                ws.Cell(5, 1).Value = "No.";
                ws.Cell(5, 2).Value = "NOMBRE";

                var dias = Enumerable.Range(0, (fechaFin - fechaInicio).Days + 1)
                                     .Select(i => fechaInicio.AddDays(i))
                                     .ToList();

                int col = 3;
                foreach (var dia in dias)
                {
                    ws.Range(5, col, 5, col + 1).Merge().Value = dia.ToString("dddd").ToUpper();
                    ws.Cell(6, col).Value = "ENTRADA";
                    ws.Cell(6, col + 1).Value = "SALIDA";
                    col += 2;
                }

                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Font.Bold = true;
                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range("A5:" + ws.Cell(6, col - 1).Address).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // ==============================
                // 🔹 LLENAR DATOS DE LA SEMANA
                // ==============================
                var colaboradores = _context.Colaboradores.OrderBy(c => c.num_empleado).ToList();
                int fila = 7;
                int num = 1;

                foreach (var c in colaboradores)
                {
                    ws.Cell(fila, 1).Value = num++;
                    ws.Cell(fila, 2).Value = c.nombre;

                    col = 3;
                    foreach (var dia in dias)
                    {
                        var registro = _context.Asistencias
                            .FirstOrDefault(a => a.num_empleado == c.num_empleado && a.fecha.Date == dia.Date);

                        if (registro != null)
                        {
                            ws.Cell(fila, col).Value = registro.hora_entrada?.ToString(@"hh\:mm");
                            ws.Cell(fila, col + 1).Value = registro.hora_salida?.ToString(@"hh\:mm");
                        }

                        ws.Range(fila, 1, fila, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Range(fila, 1, fila, col + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        col += 2;
                    }

                    fila++;
                }

                // ==============================
                // 🔹 PIE DE PÁGINA
                // ==============================
                fila += 2;
                ws.Cell(fila, 1).Value = "JORNADA DOMINGO:";
                ws.Cell(fila + 1, 1).Value = "NA - OFICINAS";
                ws.Cell(fila + 2, 1).Value = "NA - OFICINAS";

                ws.Range(fila, 1, fila + 2, 4).Style.Font.Bold = true;

                ws.Cell(fila, 9).Value = "REVISO:";
                ws.Cell(fila + 1, 9).Value = "ING. PERLA SARASAULI TLILAYATZI GONZALEZ";
                ws.Cell(fila + 2, 9).Value = "ING. FREDDY ATLAHUA GARCÍA";
                ws.Cell(fila + 2, 13).Value = "ITRH-01-F01 (01)";

                ws.Columns().AdjustToContents();

                // ==============================
                // 🔹 DESCARGAR ARCHIVO
                // ==============================
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ControlAsistencia_{fechaInicio:ddMMyyyy}_{fechaFin:ddMMyyyy}.xlsx");
                }
            }
        }
    }
    }


