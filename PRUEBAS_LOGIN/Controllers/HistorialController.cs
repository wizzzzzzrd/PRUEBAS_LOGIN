using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRUEBAS_LOGIN.Controllers
{
    public class HistorialController : Controller
    {
        public IActionResult Ventas()
        {
            return View();
        }

        public IActionResult Documentos()
        {
            return View();
        }
    }
}
