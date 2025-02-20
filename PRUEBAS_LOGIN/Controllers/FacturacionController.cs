using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRUEBAS_LOGIN.Controllers
{
    public class FacturacionController : Controller
    {
        // GET: FacturacionController/Details/5
        public IActionResult Fiscales()
        {
            return View();
        }
    }
}
