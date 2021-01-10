using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stress2
{
    class Program
    {
        private static string url = "https://beta.eduso.vn/home/test";

        static void Main(string[] args)
        {
            Console.Write("Enter max request: ");
            var max = Convert.ToInt32(Console.ReadLine());

            Stopwatch mywatch = new Stopwatch();

            Console.WriteLine("Thread Pool Execution");
            mywatch.Start();

            for (int i = 1; i <= max; i++)
            {
                //TaskFactory.StartNew(() => CreateRequest(i));
                //ThreadPool.QueueUserWorkItem(new ParameterizedThreadStart(CreateRequest));
                ThreadPool.QueueUserWorkItem(s => CreateRequest(i));
            }
            mywatch.Stop();
            Console.WriteLine("Time consumed by ProcessWithThreadPoolMethod is : " + mywatch.ElapsedTicks.ToString());
            Console.WriteLine("Stress Done");
            Console.ReadKey();
        }

        static void CreateRequest(int i)
        {
            Console.WriteLine("Request " + i + " start");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30000;

            try
            {
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("Request " + i + " done");
                }
            }
            catch (WebException e)
            {
                Console.WriteLine("Error " + i + " Occured : " + e.Message);
            }
        }
    }
}
