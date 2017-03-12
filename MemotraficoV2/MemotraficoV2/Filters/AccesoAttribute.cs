using MemotraficoV2.Models;
using System.Web.Mvc;

namespace MemotraficoV2.Filters
{
    public class AccesoAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string roles = Usuarios.Roles();
                string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Replace("Controller", "");
                string action = filterContext.ActionDescriptor.ActionName;
                if (!AccesoSistema.tieneAcceso(controller, action, roles))
                {
                    filterContext.Result = new RedirectResult("/Accesos/SinAcceso");
                }
            }
            
        }
    }
}