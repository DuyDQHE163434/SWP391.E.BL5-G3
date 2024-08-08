using System.Net.Mail;
using System.Net;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.DAO_Context
{
    public class SendEmail
    {
        traveltestContext context = new traveltestContext();
        public static bool theSendEmail(string fromEmail, string toEmail, string subject, string body, string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string username, string pass, string cf_Pass, string firstName, string lastName,
  string phoneNumber)
        {
            DAO dal = new DAO();
            try
            {
                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Port = smtpPort;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true; // Enable SSL for secure communication with the SMTP server

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subject;
                if (pass == cf_Pass && dal.IsPhoneNumberValidVietnam(phoneNumber) == true && dal.IsValidFirstnameorLastname(firstName) == true && dal.IsValidFirstnameorLastname(lastName) == true)
                {
                    mailMessage.Body = body;
                    smtpClient.Send(mailMessage);
                    return true; // Email sent successfully
                }
                else
                {
                    mailMessage.Body = "Tạo Tài Khoản Không Thành Công Vui Lòng Kiểm Tra Lại Các Thông Tin!!!";
                }


                smtpClient.Send(mailMessage);
                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false; // Email sending failed
            }
        }
    }
}
