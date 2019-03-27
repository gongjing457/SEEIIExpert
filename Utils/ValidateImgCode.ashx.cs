using System;
using System.Drawing;
using System.Web;
using System.Web.SessionState;

namespace SEEIPro.Utils
{
    /// <summary>
    /// ValidateImgCode 
    /// </summary>
    public class ValidateImgCode : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            char[] chars = "023456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int codelength = 5;
            System.Random random = new Random();

            string validateCode = string.Empty;
            for (int i = 0; i < codelength; i++)
            {
                char rc = chars[random.Next(0, chars.Length)];
                if (validateCode.IndexOf(rc) > -1)
                {
                    i--;
                    continue;
                }
                validateCode += rc;
            }
            context.Session["gong_validate_code"] = validateCode;
            CreateImage(context, validateCode);
        }

        /// <summary>
        ///create img
        /// </summary>
        /// <param name="checkCode"></param>
        private void CreateImage(HttpContext context, string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 12);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 20);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //color
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };
            Random rand = new Random();
            //font & colored char
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                Font f = new System.Drawing.Font("Microsoft Sans Serif", 11);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                g.DrawString(checkCode.Substring(i, 1), f, b, (i * 10) + 1, 0, StringFormat.GenericDefault);
            }

            //draw backgound noise
            for (int i = 0; i < 100; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(rand.Next()));
            }
            //draw border
            g.DrawRectangle(new Pen(Color.Black, 0), 0, 0, image.Width - 1, image.Height - 1);

            //output to browser
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            context.Response.ClearContent();
            context.Response.ContentType = "image/Jpeg";
            context.Response.BinaryWrite(ms.ToArray());
            g.Dispose();
            image.Dispose();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}