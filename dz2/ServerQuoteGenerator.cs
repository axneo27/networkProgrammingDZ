using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerQuoteGenerator
{

    static class RandomQuoteGenerator
    {
        private static readonly string[] quotes = new string[]
        {
            "The only limit to our realization of tomorrow is our doubts of today.",
            "The future belongs to those who believe in the beauty of their dreams.",
            "Do not wait to strike till the iron is hot, but make it hot by striking.",
            "Success usually comes to those who are too busy to be looking for it.",
            "Don't watch the clock; do what it does. Keep going."
        };

        public static string GetRandomQuote()
        {
            Random random = new Random();
            int index = random.Next(quotes.Length);
            return quotes[index];
        }
    }

    class Client
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; }
    }

    internal class Program
    {
        static Dictionary<Socket, Client> clients = new Dictionary<Socket, Client>();

        static byte[] StringToBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        static string BytesToString(byte[] bytes, int index, int length)
        {
            return Encoding.UTF8.GetString(bytes, index, length);
        }

        static Client GetClient(Socket socket)
        {
            foreach (var s in clients)
            {
                if(s.Key.RemoteEndPoint == socket.RemoteEndPoint)
                {
                    return s.Value;
                }
            }

            return null;
        }

        static void WorkWithClient(Socket client)
        {
            Task.Run(() =>
            {
                try
                {
                    var buffer = new byte[8096];

                    client.Send(StringToBytes("Enter your name and color (e.g., John - Red):"));
                    int l = client.Receive(buffer);
                    string clientData = BytesToString(buffer, 0, l);

                    string clientName = clientData.Split(" - ")[0];
                    string clientColor = clientData.Split(" - ")[1];
                    ConsoleColor userColor;
                    if (Enum.IsDefined(typeof(ConsoleColor), clientColor) == false)
                    {
                        userColor = ConsoleColor.White;
                    } else { 
                        userColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), clientColor);
                    }
                    Client clientObj = new Client { Name = clientName, Color =  userColor};

                    clients.Add(client, clientObj);
                    Console.ForegroundColor = clientObj.Color;
                    Console.WriteLine($"Client added: {clientName} - {client.RemoteEndPoint}");
                    Console.ResetColor();
                    ClientHandling(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        static void ClientHandling(Socket client)
        {
            var buffer = new byte[8096];
            while (client.Connected)
            {
                int l = client.Receive(buffer);
                string message = BytesToString(buffer, 0, l);
                if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                {
                    Console.WriteLine($"Client {client.RemoteEndPoint} disconnected. Time: {DateTime.Now}");
                    clients.Remove(client);
                    client.Close();
                    return;
                }
                var clientObj = GetClient(client);
                Console.ForegroundColor = clientObj.Color;

                Console.WriteLine($"{clientObj.Name}: {message}");
                string ran = RandomQuoteGenerator.GetRandomQuote();
                client.Send(StringToBytes(ran));

                Console.WriteLine($"Sent quote to {clientObj.Name}:");
                Console.WriteLine(ran);

                Console.ResetColor();
            }
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var ip = IPAddress.Parse("192.168.31.140");
            int port = 5555;
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var endPoint = new IPEndPoint(ip, port);
            server.Bind(endPoint);
            server.Listen(50);

            Console.WriteLine($"Server started {endPoint}");

            while(true)
            {
                try
                {
                    Console.WriteLine("Wait for client...");
                    Socket client = await server.AcceptAsync();
                    Console.WriteLine($"Client connected: {client.RemoteEndPoint}, time: {DateTime.Now}");
                    WorkWithClient(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
