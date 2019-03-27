using SEEIPro.Utils;
using System.Web.Mvc;

namespace SEEIPro
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginAuthorizationActionFilterAttribute());
        }
    }
}