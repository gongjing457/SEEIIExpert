<%@ WebHandler Language="C#" Class="Upload" %>
/**
 * SEEII All Rights Preserved
 * 2018-01-20
 * By Ryan
 *
 */
using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Globalization;
using LitJson;

public class Upload : IHttpHandler
{
    private HttpContext context;

    public void ProcessRequest(HttpContext context)
    {
        String aspxUrl = context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);

        string virtualPath = System.Configuration.ConfigurationManager.AppSettings["VirtualPath"];


        //文件保存目录路径
        //String savePath = "../attached/";
        String savePath = context.Request.QueryString["type"];//Icons

        //文件保存目录URL
        //    String saveUrl = aspxUrl + "../attached/";
        String saveUrl = virtualPath + "/" + savePath + "/";/*~/SeeiiFiles//Icons//*/

        //定义允许上传的文件扩展名
        Hashtable extTable = new Hashtable();
        extTable.Add("image", "gif,jpg,jpeg,png,bmp");
        extTable.Add("flash", "swf,flv");
        extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
        extTable.Add("file", "pdf");
        //  extTable.Add("file", "doc,pdf,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2"); 
        //最大文件大小
        int maxSize =2097152;
        this.context = context;

        HttpPostedFile imgFile = context.Request.Files["expertFile"];
        if (imgFile == null)
        {
            showError("Please choose a file!");
        }

        //  String dirPath = context.Server.MapPath(savePath);

        String dirPath = context.Server.MapPath(saveUrl);
        //F:\\SEEIExperts\\SEEIPro\\SEEIProManageApplication\\SeeiiFiles\\Icons/\\
        //E:\SEEIIBackStageReleased\\SeeiiFiles\\Icons\\

        if (!Directory.Exists(dirPath))
        {
            showError("Dir does not exist!");
        }

        String dirName = context.Request.QueryString["dir"];//image
        if (String.IsNullOrEmpty(dirName))
        {
            dirName = "file";
        }
        if (!extTable.ContainsKey(dirName))
        {
            showError("The dir name is not correct!");
        }
        String fileName = imgFile.FileName;

        String fileExt = Path.GetExtension(fileName).ToLower();

        if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
        {
            showError("Upload file size exceeds 2M!");
        }

        if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
        {
            showError("Upload file extensions are not allowed!\n Only " + ((String)extTable[dirName]) + " are allowed!");
        }


        //创建文件夹
        dirPath += dirName + "/";//F:\\SEEIExperts\\SEEIPro\\SEEIProManageApplication\\SeeiiFiles\\Icons\\image/
        saveUrl += dirName + "/";///SeeiiFiles/Icons/image/
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        String ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
        dirPath += ymd + "/";//F:\\SEEIExperts\\SEEIPro\\SEEIProManageApplication\\SeeiiFiles\\Files\\image/20180122/
        saveUrl += ymd + "/";///SeeiiFiles/Files/image/20180122/
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
        String filePath = dirPath + newFileName;


        String fileUrl = saveUrl + newFileName;///SeeiiFiles/Files/image/20180122/20180122115603_6555.jpg
        Hashtable hash = new Hashtable();
        hash["error"] = 0;
        hash["url"] = fileUrl;///SeeiiFiles/Files/image/20180122/20180122145121_8341.jpg
        try
        {
            imgFile.SaveAs(filePath);
        }
        catch (Exception e)
        {
            hash["error"] = 1;
            showError("服务器保存文件的过程中出错！错误信息如下：" + e.GetBaseException().ToString());
            throw e.GetBaseException();
        }
        if (dirName.Equals("image"))
        {
            Stream strm = imgFile.InputStream;
            System.Drawing.Image img = System.Drawing.Image.FromStream(strm);
            string imgpixel = img.Width + "*" + img.Height;
            hash["pixel"] = imgpixel;
        }
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        context.Response.Write(JsonMapper.ToJson(hash));
        context.Response.End();
    }

    private void showError(string message)
    {
        Hashtable hash = new Hashtable();
        hash["error"] = 1;
        hash["message"] = message;
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        context.Response.Write(JsonMapper.ToJson(hash));
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
