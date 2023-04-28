using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace BackendBootcamp.Logics
{
    public class EmailLogic
    {
        private static string EmailName = "";
        private static string EmailPassword = "";
        private static string EmailHost = "";
        public static int EmailPort = 587;

        public static void GetConfiguration(IConfiguration configuration)
        {
            EmailName = configuration["Email:EmailName"];
            EmailPassword = configuration["Email:EmailPassword"];
            EmailHost = configuration["Email:EmailHost"];
            EmailPort = Convert.ToInt32(configuration["Email:EmailPort"]);

        }

        public static Task SendEmail(string to, string subject, string bodyHtml)
        {
            return Task.Run(() =>
            {
                using (MimeMessage email = new MimeMessage())
                {
                    //setup email yg akan dikirim
                    email.From.Add(MailboxAddress.Parse(EmailName));
                    email.To.Add(MailboxAddress.Parse(to));
                    email.Subject = subject;
                    email.Body= new TextPart(TextFormat.Html) { Text = bodyHtml };

                    //kirim email
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Connect(EmailHost, EmailPort);
                        smtp.Authenticate(EmailName, EmailPassword);
                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }

                }
            });
        }

    }
}
