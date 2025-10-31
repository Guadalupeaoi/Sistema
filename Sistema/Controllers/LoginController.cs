using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Sistema.AppStart;
using Sistema.Models;
using Sistema.SistemaBL.Propiedades;
using Sistema.SistemaEL.Propiedades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;


namespace Sistema.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["HideLayout"] = true; // Oculta navbar y header
            return View();
           
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar usuario en la DB
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == model.Usuario
                                       && u.Contrasena == model.Contrasena);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Guardar datos del usuario en sesión
            HttpContext.Session.SetString("Usuario", usuario.Usuario);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            // Redireccionar según rol
            if (usuario.Rol == "RecursosHumanos")
                return RedirectToAction("Index", "Home"); // Puedes cambiar "Home" por el controlador de RH
            else if (usuario.Rol == "DireccionGeneral")
                return RedirectToAction("Index", "Home"); // Puedes cambiar "Home" por el controlador de DG

            // Redirección por defecto
            return RedirectToAction("Index", "Home");
        }
    }
}   

