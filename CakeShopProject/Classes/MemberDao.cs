using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopProject.Classes
{
    class Member
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string BirthDay { get; set; }
        public string Faculty { get; set; }
        public string Class { get; set; }
        public string School { get; set; }
        public string ImgSource { get; set; }
    }

    class MemberDao
    {
        public static List<Member> getData()
        {
            string Folder = AppDomain.CurrentDomain.BaseDirectory;
            string Path = $"{Folder}Resources\\Data\\Members.txt";

            MyFileManager.CheckFilePath(Path);

            var Data = File.ReadAllLines(Path);
            var result = new List<Member>();

            foreach (var Line in Data)
            {
                var Items = Line.Split('|');

                if (Items.Count() != 6)
                {
                    continue;
                }
                else { /*Do nothing*/ }

                result.Add(new Member { Name = Items[0], ID = Items[1], BirthDay = Items[2], School = Items[3], Faculty = Items[4], Class = Items[5], ImgSource = $"Resources\\Images\\{Items[1]}.jpg" });
            }

            return result;
        }

        public static string ConvertString(Member Mem)
        {
            string result = "";
            string writeline = "\r\n";

            result = $"Họ và tên: {Mem.Name}{writeline}MSSV: {Mem.ID}{writeline}Ngày sinh: {Mem.BirthDay}{writeline}Trường: {Mem.School}{writeline}Khoa: {Mem.Faculty}{writeline}Lớp: {Mem.Class}{writeline}";

            return result;
        }
    }
}
