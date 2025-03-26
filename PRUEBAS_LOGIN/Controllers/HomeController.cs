using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var usuarioJson = HttpContext.Session.GetString("usuario");

            if (string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Login", "Acceso");
            }

            // Deserializar el objeto
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Acceso");
            }

            ViewBag.NombreUsuario = usuario.Nombre;

            // Validar si el usuario es el administrador (IdUsuario == 6)
            if (usuario.IdUsuario == 6)
            {
                return RedirectToAction("IndexAdmin");
            }

            // Si no es el administrador, redirigir a la vista normal
            return View();
        }

        public IActionResult IndexAdmin()
        {
            // Obtener el usuario de la sesión
            var usuarioJson = HttpContext.Session.GetString("usuario");

            if (string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Login", "Acceso");
            }

            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

            if (usuario == null)
            {
 
                return RedirectToAction("Login", "Acceso");
            }

            // Asignar el nombre de usuario al ViewBag
            ViewBag.NombreUsuario = usuario.Nombre;

            return View();
        }
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Acceso");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}