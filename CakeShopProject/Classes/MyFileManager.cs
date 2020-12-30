using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace WeSplitProject.Classes
{
    class MyFileManager
    {
        public static void CheckFilePath(string filepath)
        {
            if (!File.Exists(filepath))
            {
                var onCreate = File.Create(filepath);
                onCreate.Close();
            }
            else { /*Do nothing*/ }
        }

        public static void CheckDictionary(string folder)
        {
            //Auto check if folder existed
            System.IO.Directory.CreateDirectory(folder);
        }


        /// newcode        
        public static bool IsDictionaryExisted(string folder)
        {
            bool result = false;
            if (System.IO.Directory.Exists(folder))
            {
                result = true;
            }
            else {/*do nothing*/}
            return result;
        }
        // ////

        public static void CheckExistedFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(filepath);
            }
        }

        public static bool IsImageFile(string fileName)
        {
            string targetExtension = System.IO.Path.GetExtension(fileName);
            bool result = false;
            if (!String.IsNullOrEmpty(targetExtension))
            {
                List<string> recognisedImageExtensions = new List<string>() { ".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff", ".ico" };
                foreach (string extension in recognisedImageExtensions)
                {
                    if (extension.Equals(targetExtension))
                    {
                        result = true;
                        break;
                    }
                }
            }
            else { /*do nothing*/ }
            return result;
        }
    }
}
