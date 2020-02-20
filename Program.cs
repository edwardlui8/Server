
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
                TcpListener tcpListener = new TcpListener(IPAddress.Parse("192.168.1.46"), 8001);
                tcpListener.Start();

                Console.WriteLine("Server running on: " + tcpListener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");

                tcpClient = tcpListener.AcceptTcpClient();

                Console.WriteLine("Accepted connection from: " + tcpClient.Client.RemoteEndPoint);

                ReceiveData();

                tcpClient.Close();
                tcpListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
            }

            Console.ReadKey();
        }

        private static void ReceiveData()
        {

            NetworkStream networkStream = tcpClient.GetStream();

            byte[] data = new byte[tcpClient.ReceiveBufferSize];
            Console.WriteLine("\nReceive buffer size {0}", data.Length);

            MemoryStream memoryStream = new MemoryStream();
            do
            {
                int bytesRead = networkStream.Read(data, 0, data.Length);
                memoryStream.Write(data, 0, bytesRead);
                Console.WriteLine("\nBytes read {0}", bytesRead);
            } while (networkStream.DataAvailable);

            Console.WriteLine("\nTotal bytes read {0}", memoryStream.Length);

            // Combined bytes
            byte[] combinedBytes = memoryStream.ToArray();

            // Filename length
            int fileNameLength = combinedBytes[0];

            // Filename
            string fileName = Encoding.ASCII.GetString(combinedBytes, 1, fileNameLength);
            Console.WriteLine("\nFile name {0}", fileName);

            // File
            int fileSize = combinedBytes.Length - 1 - fileNameLength;
            byte[] fileBytes = new byte[fileSize];
            Buffer.BlockCopy(combinedBytes, fileNameLength + 1, fileBytes, 0, fileSize);
            Console.WriteLine("\nFile size {0}", fileBytes.Length);

            string filePath = @"C:\Users\Edward\Desktop\server\" + fileName;
            File.WriteAllBytes(filePath, fileBytes);

            networkStream.Close();
        }
    }
}
