using SEEIPro.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data;
using System.IO;
using SEEIPro.Utils;
using System.Globalization;

namespace SEEIPro.Controllers
{
    [LoginAuthorizationActionFilter(isCheck = true)]
    public class ExpertsManageController : Controller
    {
        private static seeiExpertsDBEntities seeiDb = new seeiExpertsDBEntities();
        List<UnitProperty> unitList = seeiDb.UnitProperties.ToList<UnitProperty>();
        List<StorageStatu> statusList = seeiDb.StorageStatus.ToList<StorageStatu>();

        [HttpGet]
        public ActionResult Index(int? page)
        {
            int PageNum = page ?? 1;
            string searchstring = Request.QueryString["search"];
            int PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int total = 0;
            if (PageNum < 1)
            {
                PageNum = 1;
            }
            var experts = seeiDb.Experts.ToList();
            if (!String.IsNullOrEmpty(searchstring))
            {
                experts = experts.Where(e => e.eName.Contains(searchstring) || e.academicTitles.Contains(searchstring) || e.UnitDetailsOne.Contains(searchstring) || e.field.Contains(searchstring) || e.expertIntroduction.Contains(searchstring)).OrderBy(e => e.sId).ToList();
            }
            experts = GetExperts(experts, PageNum, PageSize, ref total);

            var pagedlist = new StaticPagedList<Expert>(experts, PageNum, PageSize, total);
            //通过ToPagedList扩展方法进行分页  
            //IPagedList<Expert> pagedlist = experts.ToPagedList();
            ViewBag.StatusList = statusList;
            return View(pagedlist);
        }
        private List<Expert> GetExperts(List<Expert> list, int Pagenum, int pagesize, ref int total)
        {
            var exps = (from ept in list orderby ept.sId ascending select ept).Skip((Pagenum - 1) * pagesize).Take(pagesize);
            total = list.Count();
            return exps.ToList();
        }

        /// <summary>
        /// GET: /ExpertsManage/Create添加新专家页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            ViewBag.UnitList = unitList;
            ViewBag.StatusList = statusList;
            return View();
        }

        public ActionResult Photo(int id)
        {
            if (id > 0)
            {
                Expert exp = (from expt in seeiDb.Experts where expt.sId == id select expt).FirstOrDefault();
                return View(exp);
            }
            else
            {
                Response.Write("<script>alert('请您确认要修改的专家编号!');window.location='/ExpertsManage/Index';</script>"); return null;
            }
        }

        /// <summary>
        /// save expert img icon
        /// </summary>
        /// <returns></returns> 
        public ActionResult SaveExpertImg(int id)
        {
            if (id > 0)
            {
                var expt = seeiDb.Experts.Find(id);
                expt.img = Request["imginfo"];
                seeiDb.Entry<Expert>(expt).State = EntityState.Modified;
                int res = seeiDb.SaveChanges();
                if (res > 0)
                {
                    return Json(new { status = "ok", msg = "该专家的图像已经修改并成功保存！" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { status = "no", msg = "该专家图像已经修改过了！" }, "text/html", JsonRequestBehavior.AllowGet); }
            }
            else { return Json(new { status = "error", msg = "请确认要修改的专家编号！" }, "text/html", JsonRequestBehavior.AllowGet); }
        }

        /// <summary>
        /// GET: /ExpertsManage/Upload
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Upload(int? page)
        {
            int PageNum = page ?? 1;
            int PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int total = 0;
            if (PageNum < 1)
            {
                PageNum = 1;
            }
            var experts = GetExperts(seeiDb.Experts.ToList(), PageNum, PageSize, ref total);
            var pagedlist = new StaticPagedList<Expert>(experts, PageNum, PageSize, total);
            //通过ToPagedList扩展方法进行分页  
            //IPagedList<Expert> pagedlist = experts.ToPagedList();
            ViewBag.StatusList = statusList;
            return View(pagedlist);
        }

        /// <summary>
        /// GET/filedownload/
        /// </summary>
        /// <param name="id">eid</param>
        /// <returns> FileResult</returns>
        public FileStreamResult FileDownload(int id)
        {
            string fileurl = "";
            var file_type = Request["type"];
            if (id > 0)
            {
                var expert = seeiDb.Experts.Find(id);
                switch (file_type)
                {
                    case "register":
                        fileurl = expert.registrationForm;
                        break;
                    case "certificate":
                        fileurl = expert.appointmentBook;
                        break;
                }
                string filepath = Server.MapPath(fileurl);
                string fileExt = Path.GetExtension(fileurl);
                string filedownloadname = DateTime.Now.ToString("yyyyMMddHHmmssffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                if (!System.IO.File.Exists(filepath))
                {
                    Response.Write("<script>alert('您当前下载文件不存在，请核对后重试！');window.close();</script>"); return null;
                }
                else
                {
                    return File(new FileStream(filepath, FileMode.Open), "application/pdf", filedownloadname);
                }

            }
            else
            {
                Response.Write("<script>alert('请您确认要下载文件所对应的专家编号!');window.close();</script>");
                return null;
            }

        }

        /// <summary>
        /// POST: /ExpertsManage/保存专家信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveExpert()
        {
            string eid = Request["eId"];
            string eName = Request["eName"];
            string gender = Request["gender"];
            string birthDay = Request["birthDay"];
            string imginfo = Request["imgUrl"];
            string identityNumber = Request["identityNumber"];
            string unitProperty = Request["unitProperty"];
            string unitOne = Request["unitOne"];
            string unitTwo = Request["unitTwo"];
            string unitThree = Request["unitThree"];
            string academicTitles = Request["academicTitles"];
            string field = Request["field"];
            string email = Request["email"];
            string officePhone = Request["officePhone"];
            string cellPhone = Request["cellPhone"];
            string postalAddress = Request["postalAddress"];
            string expertSources = Request["expertSources"];
            string beStatus = Request["beStatus"];
            string personalUrl = Request["personalUrl"];
            string Categories = Request["Categories"];
            string expertIntroduction = Request["expertIntroduction"];
            string expertworkingExperience = Request["expertworkingExperience"];
            string expertAchievement = Request["expertAchievement"];
            string remark = Request["remark"];
            if (eid != null && eid.Length > 0)
            {
                Expert expert = new Expert();
                expert.eId = eid;
                expert.eName = eName;
                expert.gender = gender;
                expert.birthDay = Convert.ToDateTime(birthDay);
                expert.identityNumber = identityNumber;
                expert.unitProperty = Convert.ToInt32(unitProperty);
                expert.UnitDetailsOne = unitOne;
                expert.UnitDetailsTwo = unitTwo;
                expert.UnitDetailsThree = unitThree;
                expert.academicTitles = academicTitles;
                expert.field = field;
                expert.email = email;
                expert.officePhone = officePhone;
                expert.cellPhone = cellPhone;
                expert.postalAddress = postalAddress;
                expert.expertSources = expertSources;
                expert.beStatus = Convert.ToInt32(beStatus);
                if (imginfo != null)
                {
                    expert.img = imginfo;
                }
                else
                {
                    expert.img = "/Content/xiaoyicun/obama.jpg";
                }
                expert.personalUrl = personalUrl;
                expert.Categories = Categories;
                expert.SerialNum = eid.Substring(eid.Length - 4);
                expert.expertIntroduction = expertIntroduction;
                expert.expertworkingExperience = expertworkingExperience;
                expert.expertAchievement = expertAchievement;
                expert.remark = remark;
                expert.addTime = DateTime.Now.ToLocalTime();
                seeiDb.Experts.Add(expert);
                if (seeiDb.SaveChanges() > 0)
                {
                    return Json(new { status = "ok", msg = "该专家信息已经保存成功！" }, "text/html", JsonRequestBehavior.AllowGet);      //wrong pwd
                }
                else
                {
                    return Json(new { status = "no", msg = "该专家信息可能已经存在！" }, "text/html", JsonRequestBehavior.AllowGet);      //wrong pwd
                }
            }
            else
            {
                return Json(new { status = "error", msg = "请确认要保存的专家信息后，再提交保存！" }, "text/html", JsonRequestBehavior.AllowGet);      //wrong pwd
            }
        }

        /// <summary>
        /// 获取单位性质代码
        /// </summary>
        /// <param name="unitproperty"></param>
        /// <returns></returns>
        private int unitpropertyInt(string unitproperty)
        {
            int unitInt = 0;
            for (int i = 0; i < unitList.Count; i++)
            {
                if (unitList[i].unitProperties.Equals(unitproperty))
                {
                    unitInt = unitList[i].id;
                }
            }
            return unitInt;
        }

        /// <summary>
        /// 获取入库状态码
        /// </summary>
        /// <param name="bestatus"></param>
        /// <returns></returns>
        private int statusInt(string bestatus)
        {
            int statusInt = 0;
            for (int i = 0; i < statusList.Count; i++)
            {
                if (statusList[i].beStatus.Equals(bestatus))
                {
                    statusInt = statusList[i].sid;
                }
            }
            return statusInt;
        }


        /// <summary>
        /// 专家个人详情页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            int slid = id ?? 1;
            //1.检查id
            //2.查询数据
            Expert ex = (from expt in seeiDb.Experts where expt.sId == id select expt).FirstOrDefault();
            ViewBag.UnitList = unitList;
            return View(ex);
        }

        /// <summary>
        /// 显示编辑页面 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //1.检查id
            //2.查询数据
            Expert exp = (from expt in seeiDb.Experts where expt.sId == id select expt).FirstOrDefault();
            //if (exp != null)
            //{
            //    ViewData["exptinfo"] = exp;
            //}
            ViewBag.UnitList = unitList;
            ViewBag.StatusList = statusList;
            return View(exp);
        }

        /// <summary>
        /// save edit expert
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationActionFilter(isCheck = true)]
        public void Edit(Expert expt)
        {
            //1.将实体对象加入EF对象容器中，并获取伪包装类对象  
            Expert exp = seeiDb.Experts.Find(expt.sId);
            DbEntityEntry<Expert> entry = seeiDb.Entry<Expert>(exp);
            //2.将伪包装类对象的状态设置为Detached
            entry.State = System.Data.EntityState.Detached;
            //3.修改
            exp.eId = expt.eId;
            exp.eName = expt.eName;
            exp.gender = expt.gender;
            exp.birthDay = expt.birthDay;
            exp.identityNumber = expt.identityNumber;
            exp.unitProperty = expt.unitProperty;
            exp.UnitDetailsOne = expt.UnitDetailsOne;
            exp.UnitDetailsTwo = expt.UnitDetailsTwo;
            exp.UnitDetailsThree = expt.UnitDetailsThree;
            exp.academicTitles = expt.academicTitles;
            exp.email = expt.email;
            exp.officePhone = expt.officePhone;
            exp.cellPhone = expt.cellPhone;
            exp.postalAddress = expt.postalAddress;
            exp.expertSources = expt.expertSources;
            exp.field = expt.field;
            exp.personalUrl = expt.personalUrl;
            exp.expertIntroduction = expt.expertIntroduction;
            exp.expertworkingExperience = expt.expertworkingExperience;
            exp.expertAchievement = expt.expertAchievement;
            exp.beStatus = expt.beStatus;
            exp.Categories = expt.Categories;
            exp.remark = expt.remark;
            exp.SerialNum = expt.eId.Substring(expt.eId.Length - 4);
            if (exp.expertIntroduction.Length > 1000) { Response.Write("<script>alert('专家个人简介超过最大字数限制!');</script>"); }
            else if (exp.expertworkingExperience.Length > 1000) { Response.Write("<script>alert('专家工作简历超过最大字数限制!');</script>"); }
            else if (exp.expertAchievement.Length > 1000) { Response.Write("<script>alert('专家主要工作业绩超过最大字数限制!');</script>"); }
            else
            {
                //4.设置属性状态 
                seeiDb.Entry(exp).State = System.Data.EntityState.Modified;
                //5.保存
                try
                {
                    if (seeiDb.SaveChanges() > 0)
                    {
                        Response.Write("<script>alert('该专家信息已修改!');window.location='/ExpertsManage/Index';</script>");
                    }
                    //else
                    //{
                    //    return Content("修改失败，请重试！");
                    //}
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        foreach (var item2 in item.ValidationErrors)
                        {
                            string error = string.Format("{0}:{1}\r\n", item2.PropertyName, item2.ErrorMessage);
                        }
                    }
                    throw ex.GetBaseException(); // 定位到错误属性 
                }
                catch (Exception except)
                {
                    throw except.GetBaseException();
                }
            }
        }
        /// <summary>
        /// POST: /ExpertsManage/Delete/5
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizationActionFilter(isCheck = true)]
        public ActionResult Delete(int id)
        {
            //1.check sid 
            if (id > 0)
            {
                //2. TODO:excute delete
                var entry = seeiDb.Set<Expert>().Find(id);
                seeiDb.Entry<Expert>(entry).State = System.Data.EntityState.Deleted;
                int res = seeiDb.SaveChanges();
                if (res > 0)
                {
                    return Json(new { status = "ok", msg = "已成功删除该专家的信息！" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "error", msg = "该专家信息删除失败，请稍后再试！" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "error", msg = "请您确认要删除专家的编号！" }, "text/html", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// delete appointment &&registration files
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationActionFilter(isCheck = true)]
        public ActionResult Del()
        {
            int sid = Convert.ToInt32(Request.QueryString["sid"]);
            if (sid > 0)
            {
                var expert = seeiDb.Experts.Find(sid);
                if (expert.registrationForm != null && expert.registrationForm.Length > 5)
                {
                    if (System.IO.File.Exists(Server.MapPath(expert.registrationForm)))
                    {
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(expert.registrationForm));
                            DirectoryInfo dir = new DirectoryInfo(Server.MapPath(expert.registrationForm.Substring(0, expert.registrationForm.LastIndexOf("/"))));
                            if (dir.GetFiles() == null || dir.GetFiles().Length == 0 || dir.GetDirectories() == null || dir.GetDirectories().Length == 0)
                                dir.Delete(false);
                        }
                        catch (Exception) { }
                    }
                    expert.registrationForm = "";//file not exist
                }
                if (expert.appointmentBook != null && expert.appointmentBook.Length > 5)
                {
                    if (System.IO.File.Exists(Server.MapPath(expert.appointmentBook)))
                    {
                        try
                        {
                            System.IO.File.Delete(Server.MapPath(expert.appointmentBook));
                            DirectoryInfo dir = new DirectoryInfo(Server.MapPath(expert.appointmentBook.Substring(0, expert.appointmentBook.LastIndexOf("/"))));
                            if (dir.GetFiles() == null || dir.GetFiles().Length == 0 || dir.GetDirectories() == null || dir.GetDirectories().Length == 0)
                                dir.Delete(false);
                        }
                        catch (Exception) { }
                    }
                    expert.appointmentBook = "";//file not exist
                }
                seeiDb.Entry<Expert>(expert).State = EntityState.Modified;
                int res = seeiDb.SaveChanges();
                if (res > 0)
                {
                    expert = null;
                    return Json(new { status = "ok" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                { return Json(new { status = "no" }, "text/html", JsonRequestBehavior.AllowGet); }
            }
            else
            { return Json(new { status = "error" }, "text/html", JsonRequestBehavior.AllowGet); }
        }

        /// <summary>
        /// 专家列表导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult OutToExcel()
        {
            string sheetname = ConfigurationManager.AppSettings["Sheet"];
            List<Expert> list = seeiDb.Experts.ToList();
            List<ExpertInfo> info_list = GetPropertiesString(list);
            string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";//获取当前时间
            DataTable dt = ExcelUtil.ConvertToDataTable(info_list);
            string[] headers = {"序号","专家编号","姓名","性别","单位性质","单位信息","技术职称","从事领域","邮箱","办公电话","手机号码",
                                   "通信地址","相关主页","业务类别","个人简介","主要工作经历","主要工作业绩","备注说明","入库状态","添加时间"};
            string[] cellKes = {"sId","eId","eName","gender","unitporetyString","UnitDetailsOne","academicTitles","field","email","officePhone","cellPhone",
                                   "postalAddress","personalUrl","Categories","expertIntroduction","expertworkingExperience","expertAchievement","remark","bestatusString","addTime"};
            MemoryStream ms = ExcelUtil.Export(dt, sheetname, headers, cellKes);

            #region 将excel文件保存到服务器指定路径
            //string xlsname = ConfigurationManager.AppSettings["Excel"];
            // byte[] data = ms.ToArray();//Encoding.UTF8.GetBytes();
            // string filePath = Server.MapPath("~/Content/xiaoyicun/" + xlsname + ".xls");
            // FileManager.WriteBuffToFile(data, filePath);  
            #endregion

            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd .ms-excel", Url.Encode(filename));
        }

        /// <summary>
        /// 替换字段属性
        /// </summary>
        /// <param name="li"></param>
        /// <returns></returns>
        private List<ExpertInfo> GetPropertiesString(List<Expert> li)
        {
            List<ExpertInfo> infolist = new List<ExpertInfo>();
            ExpertInfo exptmp;
            foreach (var item in li)
            {
                exptmp = new ExpertInfo();
                exptmp.sId = item.sId; exptmp.eId = item.eId;
                exptmp.eName = item.eName; exptmp.gender = item.gender;
                exptmp.birthDay = item.birthDay; exptmp.identityNumber = item.identityNumber;
                exptmp.UnitDetailsOne = item.UnitDetailsOne;
                exptmp.UnitDetailsTwo = item.UnitDetailsTwo; exptmp.UnitDetailsThree = item.UnitDetailsThree;
                exptmp.field = item.field; exptmp.academicTitles = item.academicTitles;
                exptmp.email = item.email; exptmp.officePhone = item.officePhone;
                exptmp.cellPhone = item.cellPhone; exptmp.postalAddress = item.postalAddress;
                exptmp.expertSources = item.expertSources;
                exptmp.img = item.img; exptmp.personalUrl = item.personalUrl;
                exptmp.registrationForm = item.registrationForm; exptmp.Categories = item.Categories;
                exptmp.appointmentBook = item.appointmentBook; exptmp.SerialNum = item.SerialNum;
                exptmp.remark = item.remark; exptmp.expertIntroduction = item.expertIntroduction;
                exptmp.expertworkingExperience = item.expertworkingExperience; exptmp.expertAchievement = item.expertAchievement;
                exptmp.addTime = item.addTime;
                #region 替换字段
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i].id == item.unitProperty)
                    {
                        exptmp.unitporetyString = unitList[i].unitProperties;
                    }
                }
                for (int i = 0; i < statusList.Count; i++)
                {
                    if (statusList[i].sid == item.beStatus)
                    {
                        exptmp.bestatusString = statusList[i].beStatus;
                    }
                }
                #endregion
                infolist.Add(exptmp);
                exptmp = null;
            }
            return infolist;
        }


        public ActionResult UploadAppointment()
        {
            int esid = Convert.ToInt32(Request.QueryString["id"]);
            string relativepath = Request["dirpath"];
            if (esid > 0)
            {
                var expt = seeiDb.Experts.Find(esid);
                expt.appointmentBook = relativepath;
                seeiDb.Entry<Expert>(expt).State = EntityState.Modified;
                int rest = seeiDb.SaveChanges();
                if (rest > 0)
                {
                    expt = null; return Json(new { status = "ok" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                { return Json(new { status = "no" }, "text/html", JsonRequestBehavior.AllowGet); }
            }
            else
            { return Json(new { status = "error" }, "text/html", JsonRequestBehavior.AllowGet); }
        }

        public ActionResult UploadRegister()
        {
            int esid = Convert.ToInt32(Request.QueryString["id"]);
            string relativepath = Request["dirpath"];
            if (esid > 0)
            {
                var expt = seeiDb.Experts.Find(esid);
                expt.registrationForm = relativepath;
                seeiDb.Entry<Expert>(expt).State = EntityState.Modified;
                int rest = seeiDb.SaveChanges();
                if (rest > 0)
                {
                    expt = null; return Json(new { status = "ok" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                { return Json(new { status = "no" }, "text/html", JsonRequestBehavior.AllowGet); }
            }
            else
            { return Json(new { status = "no" }, "text/html", JsonRequestBehavior.AllowGet); }
        }

    }
}
