using System;
using System.Web.Mvc; 
using SEEIPro.Utils;
using System.Data;
using System.Collections.Generic;
using System.Web.Configuration;
using SEEIPro.Models;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace SEEIPro.Controllers
{

    public class BackstageController : Controller
    {

        private static seeiExpertsDBEntities seeiiexpertDB = new seeiExpertsDBEntities();

        //获取Web.config文件中数据库连接的配置信息
        //   public static readonly string connstr = ConfigurationManager.ConnectionStrings["seeiExpertsDBEntities"].ConnectionString; 


        //
        // GET: /Home/
        public ActionResult Login()
        {
            return View();
        }

        [LoginAuthorizationActionFilter(isCheck = true)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [LoginAuthorizationActionFilter(isCheck = true)]
        public ActionResult ValidatePassword()
        {
            string pass = Request["pwd"];
            string newpwd = Request["newpwd"];
            if (pass != null && pass.Length > 0)
            {
                Staff loginmodel = (Staff)Session["loginModel"]; 
                if (pass == loginmodel.passWord)
                {
                    Staff user = seeiiexpertDB.Staffs.Find(loginmodel.sId);
                    DbEntityEntry<Staff> entry = seeiiexpertDB.Entry<Staff>(user);
                    entry.State = EntityState.Detached;
                    user.passWord = newpwd;
                    seeiiexpertDB.Entry(user).State = EntityState.Modified;
                    if (seeiiexpertDB.SaveChanges() > 0)
                    {
                        Session["loginModel"] = null;
                        return Json(new { status = "ok", msg = "密码已经修改成功，请您用新密码登录！" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "error", msg = "新密码保存错误！" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", msg = "您输入的账户密码有误！" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "error", msg = "请确认您的账户密码！" }, "text/html", JsonRequestBehavior.AllowGet);
            }
        }

        //
        // POST: /Home/Handler
        [HttpPost]
        public ActionResult Handler()
        {
            string username = Request["username"];
            string enpass = Request["passpwd"];
            string valicode = Request["validatecode"];
            string validatecode = Session["gong_validate_code"].ToString();
            if (validatecode.Equals(valicode))
            {
                string privatekeypath = AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["RSAPrivateKey"];
                string privatekey = System.IO.File.ReadAllText(privatekeypath);
                string plainpwd = RSASecurity.DecryptRSA(enpass, privatekey);
                if (!string.IsNullOrEmpty(privatekey))
                {
                    Staff userinfo = (from u in seeiiexpertDB.Staffs where u.userName == username && u.passWord == plainpwd select u).FirstOrDefault();
                    if (userinfo != null)
                    {
                        Session["loginModel"] = userinfo;
                        Session["name"] = userinfo.name;
                        return Json(new { status = "ok", msg = "登陆成功，欢迎回来！" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "error", msg = "您输入的用户名或密码有误，请核对后重新输入！" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
                else { return Json(new { status = "error", msg = "密钥错误或者不存在，请稍后再试！" }, "text/html", JsonRequestBehavior.AllowGet); }
            }
            else { return Json(new { status = "error", msg = "您输入的验证码有误，请核对后重新输入！" }, "text/html", JsonRequestBehavior.AllowGet); }
        }

        /// <summary>
        /// return Key
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRsaKey()
        {
            var kp = new Dictionary<string, string>();
            kp = RSASecurity.CreateRsaPair();
            return Json(new { status = "ok", publickey = kp["PUBLIC"].ToString() }, "text/html", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Quit()
        {
            if (Session["loginModel"] == null)
            {
                return Json(new { status = "ok", msg = "您已经退出系统，即将返回至智库首页！" }, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["loginModel"] = null;
                return Json(new { status = "ok", msg = "您已经退出系统，即将返回至智库首页！" }, "text/html", JsonRequestBehavior.AllowGet);
            }
        }
        protected override void HandleUnknownAction(string actionName)
        {

            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch (InvalidOperationException ieox)
            {

                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";

                ViewData["exMessage"] = ieox.Message;

                this.View("Error").ExecuteResult(this.ControllerContext);

            }
        }
    }
}
