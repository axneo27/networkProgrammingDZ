using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string ipAddress = "192.168.31.121";
            int port = 5555;
            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(serverEndPoint);

            try
            {
                while (client.Connected)
                {
                    // send 
                    Console.Write("Enter message: ");
                    string? message = Console.ReadLine();
                    message ??= "Empty message";

                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    client.Send(messageBytes);

                    // receive 
                    byte[] buffer = new byte[8096];
                    int l = client.Receive(buffer);
                    string serverResponse = Encoding.UTF8.GetString(buffer, 0, l);

                    Console.WriteLine($"О {DateTime.Now.ToString("HH:mm")} від {client.RemoteEndPoint} отримано рядок: {serverResponse}");

                    if (string.Equals(message.ToLower(), "date") || string.Equals(message.ToLower(), "time"))
                    {
                        client.Shutdown(SocketShutdown.Both);
                        Console.WriteLine("Exiting client...");
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}