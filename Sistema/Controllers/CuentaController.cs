using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Sistema.Models;
using Sistema.SistemaBL.Propiedades;
using Sistema.SistemaEL.Propiedades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Sistema.Controllers
{
    public class CuentaController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            cuentaModel model = new cuentaModel();

            try
            {

            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(cuentaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cuentaBL accountObj = new cuentaBL();
                    List<cuentaEL> usuarioList = accountObj.InicioSesion(model.nombreUsuario, model.contraseña);

                    if (usuarioList.Any())
                    {
                        var user = usuarioList.First();

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.nombreUsuario),
                    // Puedes agregar más claims aquí (como rol, email, etc.)
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddHours(3)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                        );

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al iniciar sesión");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Cuenta");
        }

    }
}
