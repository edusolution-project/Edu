using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
namespace KiemTraNhieuThe
{
    class Program
    {
        static void Main(string[] args)
        {

            List<int> data = new List<int>() { 0,33,44,55 };
            List<Model> models = new List<Model>()
            {
                new Model()
                {
                    ID = 1,
                    Name = "Hoàng Long 1"
                },
                new Model()
                {
                    ID = 2,
                    Name = "Hoàng Long 2"
                },
                new Model()
                {
                    ID = 3,
                    Name = "Hoàng Long 3"
                }
            };
            var xxx = models.Where(o => data.IndexOf(o.ID) > -1);
            if (xxx == null || xxx.Count() <= 0) Console.WriteLine("null");
            else
            foreach(var x in xxx)
            {
                Console.WriteLine(x.Name);
            }
//            string test = "<div style=\"width:50%;float:left\">" +
//   "<static-layout code=\"CAdv\" id= \"vswLogo\"> Logo </static-layout>" +
//"</div>" +
//"<div style=\"width:50%;float:left\">" +
//    "<static-layout code=\"CMenu\" id=\" vswNav\"> MenuTop </static-layout>" +
//"</div>" +
//"<div style=\"width:100%;float:left\">" +
//    "<dynamic-layout code=\"CAdv\" id=\"vswNav \"> MAin </dynamic-layout>" +
//"</div>";
//            string regexElement = @"<\s*static-layout[^>]*>(.*?)\<\s*\/\s*static-layout>|<\s*dynamic-layout[^>]*>(.*?)\<\s*\/\s*dynamic-layout>";
//            string regexID = @"id\s*=\s*\""(.*?)""";
//            var match = Regex.Replace(test,regexElement,"");
//            Console.WriteLine(match);
//            //foreach(var item in match)
//            //{
//            //    Console.WriteLine(item.ToString().Trim() + " -" + (item.ToString().Trim().IndexOf("static-layout") > -1));
//            //    Console.WriteLine(item.ToString().Trim().IndexOf("dynamic-layout") > -1);
//            //    var data = Regex.Match(item.ToString().Trim(), regexID);
//            //    Console.WriteLine(data.Value.Replace(@"id=", "").Replace(@"""","").Trim());
                
//            //}
            Console.ReadKey();
        }
    }
    public class Model
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
