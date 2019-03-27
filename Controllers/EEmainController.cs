using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using PagedList;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Net;
using SEEIPro.Models;
using iText.IO.Image;
using iText.Kernel.Geom;

namespace SEEIPro.Controllers
{
    public class EEmainController : Controller
    {
        seeiExpertsDBEntities seeiDB = new seeiExpertsDBEntities();
        List<Expert> expts = new List<Expert>();
        //
        // GET: /MainPage/

        public ActionResult HomePage()
        {
            expts = seeiDB.Experts.ToList<Expert>();
            ViewData["total"] = expts.Count;
            return View();
        }

        public ActionResult ExpertProfile(int? page)
        {
            int PageNum = page ?? 1;
            int PageSize = int.Parse(ConfigurationManager.AppSettings["IndustrailPageSize"]);
            int total = 0;
            if (PageNum < 1)
            {
                PageNum = 1;
            }
            expts = seeiDB.Experts.ToList<Expert>();
            expts = GetExperts(expts, PageNum, PageSize, ref total);
            var pagedlist = new StaticPagedList<Expert>(expts, PageNum, PageSize, total);
            return View(pagedlist);
        }


        private List<Expert> GetExperts(List<Expert> list, int Pagenum, int pagesize, ref int total)
        {
            var exps = (from ept in list orderby ept.sId ascending select ept).Skip((Pagenum - 1) * pagesize).Take(pagesize);
            total = list.Count();
            return exps.ToList();
        }


        public ActionResult About()
        {
            return View();
        }
        public ActionResult Tools()
        {
            return View();
        }

        [HttpPost]
        public FileResult CreatePDF()
        {
            string[] urlLinks = new string[12];
            urlLinks = (string[])Request.Form["urls"].Split(',');
            string filepath = "E:\\demo.pdf";
            if (urlLinks[0].Length > 10)
            {
                System.Drawing.Image img = getOnineImage(urlLinks[0]);

               PdfWriter pdfwriter = new PdfWriter(filepath); 
                PdfDocument pdf = new PdfDocument(pdfwriter);
                iText.Layout.Document document = new iText.Layout.Document(pdf,PageSize.A4);
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                document.Add(new Paragraph("This pdf is created by gongjing457!")).Add(new Image(ImageDataFactory.Create(img, System.Drawing.Color.White))).SetFont(font);
                document.Close();
                return File(new FileStream(filepath, FileMode.Open), "application/pdf", "demo");

            }
            else
            {
                return File(new FileStream(filepath, FileMode.Open), "application/pdf", "demo");
            }

        }

        private System.Drawing.Image getOnineImage(string url)
        {
            System.Drawing.Image image = null;
            try
            {
                WebRequest webrequest = WebRequest.Create(url);
                using (WebResponse response = webrequest.GetResponse())
                {
                    response.GetResponseStream();
                    if (response != null)
                    {
                        image = System.Drawing.Image.FromStream(response.GetResponseStream());
                    }
                }
            }
            catch (Exception)
            {
                image = null;
            }
            return image;
        }
    }
}
