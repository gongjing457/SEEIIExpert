using SEEIPro.Models;
using System.Web.Mvc;

namespace SEEIPro.Utils
{
    public class AuthorizationActionFilter : ActionFilterAttribute
    {
        public bool isCheck { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (isCheck)
            {
                Staff user = filterContext.HttpContext.Session["loginModel"] as Staff;
                if (user.privilegeLevel < 3)
                {
                    filterContext.HttpContext.Response.Write("<script>alert('对不起,你没有此操作的权限!');</script>");
                }
            }
        }

    }
}