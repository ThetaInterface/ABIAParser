using System.Net;
using System.Net.Mail;

using static ABIAParser.EProperty;

namespace ABIAParser;

public static class MailManager
{
    private static readonly Dictionary<EProperty, string> config = [];

    public static void Init(Dictionary<EProperty, string> config)
    {
        foreach (KeyValuePair<EProperty, string> pair in config)
            MailManager.config.Add(pair.Key, pair.Value);
    }

    public static bool SendMail(string body)
    {
        try
        {
            MailMessage mail = new(from: config[SendFrom], to: config[SendTo])
            {
                Subject = "ABIAParser",
                Body = DateTime.Now.ToString() + body
            };

            SmtpClient client = new(host: config[SMTPServer], port: int.Parse(config[SMTPPort]))
            {
                Credentials = new NetworkCredential(userName: config[SMTPUser], password: config[SMTPUserPassword]),
                EnableSsl = true
            };

            client.Send(mail);

            mail.Dispose();
            client.Dispose();

            return true;
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Mail Send Error) {ex.Message}");

            return false;
        }
    }
}