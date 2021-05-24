using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{       
    class Program
    {
        // aceasta aplicatie este folosita pentru a receptiona date de la proces si pentru a le trimite mai departe catre WebAPI
        
        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER to start the MONITOR process...");
            Console.ReadLine();
            Comm.Receiver receiver = new Comm.Receiver("127.0.0.1", 3000);
            receiver.DataReceived += ReceivedSomeData;
            receiver.StartListen();
        }

        // acest eveniment de declanseaza atunci cand au fost receptionate date
        private static void ReceivedSomeData(object sender, EventArgs e)
        {            
            //Console.WriteLine(sender.ToString());
            switch (Convert.ToInt32(sender.ToString()))
            {
                case 0:
                    Console.WriteLine("RESET");
                    break;
                case 1:
                    Console.WriteLine("Sistemul este pornit, valva K1 este deschisa!");
                    break;
                case 2:
                    Console.WriteLine("Nivelul apei a declansat senzorul B2, pompa M1 este pornita!");
                    break;
                case 3:
                    Console.WriteLine("Au trecut 3s de cand senzorul B2 a fost activat, pompa M2 este pornita!");
                    break;
                case 4:
                    Console.WriteLine("Nivelul apei scade sub B1, pompele M1 si M2 se opresc!");
                    break;
                case 5:
                    Console.WriteLine("Nivelul apei a declansat senzorul B3, valva K1 trebuie inchisa!"); 
                    break;
                case 6:
                    Console.WriteLine("Pompa M1/ M2 s - a defectat, a fost inlocuita de pompa M3!");
                    break;
                case 7:
                    Console.WriteLine("Sistemul este oprit!");
                    break;
                    //case 0:
                    //    Console.WriteLine("Totul este oprit!");
                    //    break;
                    //case 1:
                    //    Console.WriteLine("Galben intermitent aprins");
                    //    break;
                    //case 2:
                    //    Console.WriteLine("Galben intermitent stins");
                    //    break;
                    //case 3:
                    //    Console.WriteLine("Rosu masini, trec oameni");
                    //    break;
                    //case 4:
                    //    Console.WriteLine("Galben masini, o sa treaca oameni...");
                    //    break;
                    //case 5:
                    //    Console.WriteLine("Verde masini, NU trec oameni, pericol mare!!!");
                    //    break;
            }

            // metoda de mai jos ia datele primite, creaza un obiect de tipul Stamp si il trimite catre WebAPI care il va stoca. 

            var postData = new Stamp(sender.ToString(), DateTime.Now);
            PutData(postData);
        }

    

        public static void PutData(Stamp PostData)
        {          
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create("http://localhost:49570/api/simulator");
            // Set the Method property of the request to POST.  
            request.Method = "POST";

            // Create POST data and convert it to a byte array.  
            string postData = JsonConvert.SerializeObject(PostData);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;

            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();

            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.  
            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Display the content.  
                Console.WriteLine(responseFromServer);
            }

            // Close the response.  
            response.Close();
        }
    }
}
