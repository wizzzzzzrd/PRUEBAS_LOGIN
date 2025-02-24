using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PRUEBAS_LOGIN.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // Verificamos si "userId" existe en la sesión
            int? userId = session.GetInt32("userId");

            if (!userId.HasValue)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Acceso", null);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
