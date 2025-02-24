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
