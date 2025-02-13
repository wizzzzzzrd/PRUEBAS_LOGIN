using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PRUEBAS_LOGIN.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Accedemos al contexto HTTP directamente desde el filterContext
            var session = filterContext.HttpContext.Session;

            // Verificamos si la sesión contiene el valor "usuario"
            if (session.GetString("usuario") == null)
            {
                // Si no hay sesión, redirigimos al login
                filterContext.Result = new RedirectToActionResult("Login", "Acceso", null);
            }

            base.OnActionExecuting(filterContext);
        }
    }   
}
