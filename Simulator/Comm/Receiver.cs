using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Comm
{
    public class Receiver
    {
        TcpListener _receiver;
        public EventHandler DataReceived;

        public Receiver(string IpAddress, int PortNumber)
        {
            _receiver = new TcpListener(IPAddress.Parse(IpAddress), PortNumber);
        }

        public void StartListen()
        {
            try
            {
                _receiver.Start();

                Byte[] bytes = new Byte[256];

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    //Se creaza un TCP client nou
                    TcpClient client = _receiver.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // se obtine o noua referinta catre streamul folosit pentru comunicare
                    NetworkStream stream = client.GetStream();

                    int i;

                    //se citesc datele primite
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {                      
                        var importantValue = bytes[0];
                        if(DataReceived!=null)
                        {
                            DataReceived(importantValue, null);
                        }                        
                    }

                    //la final se inchide conexiunea
                    client.Close();
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            
        }
    }
}
