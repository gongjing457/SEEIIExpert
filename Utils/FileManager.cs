using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SEEIPro.Utils
{
    public class FileManager
    {
        /// <summary>  
        /// 写字节数组到文件  
        /// </summary>  
        /// <param name="buff"></param>  
        /// <param name="filePath"></param>  
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            WriteBuffToFile(buff, 0, buff.Length, filePath);
        }
        /// <summary>  
        /// 写字节数组到文件  
        /// </summary>  
        /// <param name="buff"></param>  
        /// <param name="offset">开始位置</param>  
        /// <param name="len"></param>  
        /// <param name="filePath"></param>  
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(buff, offset, len);
            writer.Flush();
            writer.Close();
            output.Close();
        }
    }
}