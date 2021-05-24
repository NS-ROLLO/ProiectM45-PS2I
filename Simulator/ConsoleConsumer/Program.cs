using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleConsumer
{
    // aceasta aplicatie este utilizata pentru a consuma date care sunt disponibile la nivelul aplicatiei WebAPI
    // aplicatia doar citeste date, nu face nimic cu ele, afiseaza doar cate pachete de date a citit, aici va trebui sa vedeti voi cum doriti sa va consumati pachetele de date
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER to start ...");
            Console.ReadLine();

            while(true)
            {
                var data = GetData();
                Console.WriteLine($"Au fost primite {data.Count} pachete de date!");
                System.Threading.Thread.Sleep(5000);
            }
        }


        public static List<Stamp> GetData()
        {
            string html = string.Empty;
            string url = @"http://localhost:49570/api/simulator";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            var userData = JsonConvert.DeserializeObject<List<Stamp>>(html);
           
            return userData;
        }        
    }
}
