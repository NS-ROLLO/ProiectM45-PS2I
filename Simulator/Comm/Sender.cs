using System;
using System.Net.Sockets;

namespace Comm
{
    public class Sender
    {
		TcpClient _sender;
		
		public Sender(string IpAddress, int PortNumber)
		{			
			_sender = new TcpClient(IpAddress,PortNumber);
		}
		public void Send(byte ValueToSend)
		{
			try
			{
				NetworkStream nwStream = _sender.GetStream();
				byte[] bytesToSend = new byte[4];
				bytesToSend[0] = ValueToSend;
				nwStream.Write(bytesToSend, 0, bytesToSend.Length);				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
