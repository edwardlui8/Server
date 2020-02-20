
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static TcpClient tcpClient = null;

        static void Main(string[] args)
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("192.168.1.46");

                TcpListener tcpListener = new TcpListener(ipAd, 8001);

                tcpListener.Start();

                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" +
                                  tcpListener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                tcpClient = tcpListener.AcceptTcpClient();

                ReceiveData();
                tcpClient.Close();
                tcpListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

            Console.ReadKey();
        }

        private async static void ReceiveData()
        {

            NetworkStream networkStream = null;

            networkStream = tcpClient.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] data = new byte[tcpClient.ReceiveBufferSize];
            MemoryStream memoryStream = new MemoryStream();
            do
            {
                networkStream.Read(data, 0, data.Length);
                memoryStream.Write(data, 0, data.Length);
            } while (networkStream.DataAvailable);

            Console.WriteLine("\nFinished Writing");
            //networkStream.Read(data, 0, tcpClient.ReceiveBufferSize);

            string fileName = @"C:\Users\Edward\Desktop\alfie2.jpg";
            File.WriteAllBytes(fileName, memoryStream.ToArray());

            byte[] response = asen.GetBytes("Hello client"); ;
            networkStream.Write(response, 0, response.Length);

            //s.Send(asen.GetBytes("The string was recieved by the server."));
            Console.WriteLine("\nSent Acknowledgement");

            networkStream.Close();
        }
    }
}
