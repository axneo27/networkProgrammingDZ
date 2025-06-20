using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client2
{
    internal class Program
    {
        static byte[] StringToBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        static string BytesToString(byte[] bytes, int index, int length)
        {
            return Encoding.UTF8.GetString(bytes, index, length);
        }

        static void SendRecieveMessages(Socket client)
        {
            Task.Run(() =>
            {
                try
                {
                    var buffer = new byte[8096];
                    while(true)
                    {
                        string message = Console.ReadLine() ?? "";
                        var bytes = StringToBytes(message);
                        client.Send(bytes);
                        if (message.ToLower() == "exit" || string.IsNullOrEmpty(message))
                        {
                            Console.WriteLine("Exiting...");
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            Console.WriteLine("Disconnected from server.");

                            return;
                        }

                        int l = client.Receive(buffer);
                        string serverMessage = BytesToString(buffer, 0, l);
                        Console.WriteLine("Server: " + serverMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var ip = IPAddress.Parse("192.168.31.140");
            int port = 5555;
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var endPoint = new IPEndPoint(ip, port);

            try
            {
                client.Connect(endPoint);
                Console.WriteLine("Connected");

                var buffer = new byte[8096];
                int l = client.Receive(buffer);
                string message = BytesToString(buffer, 0, l);
                Console.WriteLine("Server: " + message);

                string name = Console.ReadLine() ?? "unknown";
                var bytes = StringToBytes(name);
                client.Send(bytes);

                SendRecieveMessages(client);

                while(client.Connected)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
