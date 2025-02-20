using Microsoft.AspNetCore.Mvc;

namespace PRUEBAS_LOGIN.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Perfil()
        {
            return View();
        }

    }
}
