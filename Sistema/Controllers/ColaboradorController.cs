using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Sistema.AppStart;
using Sistema.Entidad;
using Sistema.Funciones;
using Sistema.Models;
using Sistema.SistemaBL.Propiedades;
using Sistema.SistemaDL.Propiedades;
using System.Linq;

namespace Sistema.Controllers
{

    
    public class ColaboradorController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ColaboradorController(ApplicationDbContext context)
        {
            _context = context;
        }

       

        public async Task<IActionResult> Index(string? q)
        {
            var colaboradores = from c in _context.Colaboradores
                                select c;

            if (!string.IsNullOrEmpty(q))
            {
                q = q.ToLower();
                colaboradores = colaboradores.Where(c =>
                    c.num_empleado.ToString().Contains(q)||
                    c.nombre.ToLower().Contains(q) ||
                    c.apellido.ToLower().Contains(q) ||
                    c.area.ToLower().Contains(q)
                );
            }

            return View(await colaboradores.ToListAsync());
        }



        [HttpGet]
            public ActionResult Create()
            {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ColaboradorModel colaborador)
        {
            Console.WriteLine(">>> Entró al método Create POST");

            if (ModelState.IsValid)
            {
                _context.Colaboradores.Add(colaborador);
                _context.SaveChanges();
                Console.WriteLine(">>> Colaborador guardado correctamente");
                return RedirectToAction("Index");
            }

            Console.WriteLine(">>> Modelo no válido");
            return View(colaborador);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.num_empleado == id);

            if (colaborador == null)
                return NotFound();

            return View(colaborador);
        }
       
        // POST: Colaborador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador != null)
            {
                _context.Colaboradores.Remove(colaborador);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var colaborador = _context.Colaboradores.Find(id);
            if (colaborador == null)
            {
                return NotFound();
            }
            return View(colaborador);
        }

        // POST: Colaborador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ColaboradorModel colaborador)
        {
            if (id != colaborador.num_empleado)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Update(colaborador);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(colaborador);
        }

    } }

