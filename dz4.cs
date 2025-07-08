using System.Net;
using System.Net.Mail;

namespace dz4 {

    public class MailService
    {
        private SmtpClient smtpClient;
        private string host;
        private int port;
        private string email;
        private string password;

        public MailService(string host, int port, string email, string password)
        {
            this.host = host;
            this.port = port;
            this.email = email;
            this.password = password;
            smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(email, password);
        }

        public async Task SendEmailAsync(MailMessage message)
        {
            await smtpClient.SendMailAsync(message);
        }

        public async Task SendEmailAsync(string[] to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<Attachment>(), isHtml);
        }

        public async Task SendEmailAsync(string[] to, string subject, string body, List<Attachment> attachments, bool isHtml = false)
        {
            var message = new MailMessage();
            message.From = new MailAddress(email);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            foreach (var t in to)
            {
                message.To.Add(t);
            }

            foreach (var a in attachments)
            {
                message.Attachments.Add(a);
            }

            await SendEmailAsync(message);
        }
    }

    class Program
    {

        static void AddToAttachments(List<Attachment> attachments, string filePath)
        {
            if (File.Exists(filePath))
            {
                attachments.Add(new Attachment(filePath));
            }
            else
            {
                Console.WriteLine($"File {filePath} does not exist.");
            }
        }

        static void GetAttachmentsFromUser(List<Attachment> attachments)
        {
            string r;
            while (true)
            {
                Console.WriteLine("Input attachment path or press 1 to send an email:");
                r = Console.ReadLine() ?? "";
                if (r == "1") { break; }
                else { AddToAttachments(attachments, r); }
            }
            Console.WriteLine("Sending an email...");
        }

        static void Main(string[] args)
        {
            string myEmail = "";
            string myPassword = ""; 
            
            string receiverEmail;
            Console.Write("Input receiver email: ");
            receiverEmail = Console.ReadLine() ?? "";

            string subject;
            Console.Write("Input email subject: ");
            subject = Console.ReadLine() ?? "";

            string filePath;
            Console.Write("Input text file path: ");
            filePath = Console.ReadLine() ?? "";

            string content = "";
            bool isHtml = false;
            try
            {
                content = File.ReadAllText(filePath);
                isHtml = Path.GetExtension(filePath).ToLower() == ".html";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            List<Attachment> attachments = new List<Attachment>();
            GetAttachmentsFromUser(attachments);

            MailService mailService = new MailService("smtp.gmail.com", 587, myEmail, myPassword);
            mailService.SendEmailAsync(new string[] {receiverEmail}, subject, content, attachments, isHtml).Wait();
        }
    }
}
