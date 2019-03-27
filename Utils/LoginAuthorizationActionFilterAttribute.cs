using System.Web.Mvc;

namespace SEEIPro.Utils
{
    public class LoginAuthorizationActionFilterAttribute : ActionFilterAttribute
    {
        public bool isCheck { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (isCheck)
            {
                if (filterContext.HttpContext.Session["loginModel"] == null)
                {
                    filterContext.HttpContext.Response.Write("<script>alert('请您先登录系统后，再继续进行操作！');window.location='/Backstage/Login';</script>");
                }
            }
        }
    
    }
}