using System;
using System.Net.Mail;

namespace BL.Services
{
    public class MailService
    {
        private const string Password = "3A1640789334308051";
        private const string UserName = "dronesdeliverysystem";
        
        public static void sendMail(string mailTo, string message)
        {
            try
            {mailTo = "seyal613@gmail.com";
                SmtpClient mySmtpClient = new SmtpClient("my.smtp.exampleserver.net");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                    System.Net.NetworkCredential(UserName, Password);
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("test@example.com", UserName);
                MailAddress to = new MailAddress(mailTo, "Target");
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = "Mail Delivery System";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "<b>" + message + "</b><br>using <b>HTML</b>.";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }
            catch (Exception ex)
            {
                // error was raised
            }
        }
    }
}