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
            // Recuperar el objeto oUsuario de la sesión
            var oUsuarioJson = HttpContext.Session.GetString("usuario");

            if (string.IsNullOrEmpty(oUsuarioJson))
            {
                // Si no hay sesión, redirigir al login
                return RedirectToAction("Login", "Acceso");
            }

            // Deserializar el objeto oUsuario
            var oUsuario = JsonConvert.DeserializeObject<Usuario>(oUsuarioJson);

            // Verificar el valor de oUsuario.Nombre
            if (string.IsNullOrEmpty(oUsuario.Nombre))
            {
                Console.WriteLine("El nombre del usuario está vacío o nulo.");
            }
            else
            {
                Console.WriteLine($"Nombre del usuario: {oUsuario.Nombre}");
            }

            // Pasar el nombre del usuario a la vista
            ViewBag.NombreUsuario = oUsuario.Nombre;

            return View();
        }

        

        public IActionResult CerrarSesion()
        {
            // Elimina todos los datos almacenados en la sesión
            HttpContext.Session.Clear();

            // Redirige a la página de login
            return RedirectToAction("Login", "Acceso");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
