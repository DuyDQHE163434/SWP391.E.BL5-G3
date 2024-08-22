using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.DAO_Context
{
    public class DAO
    {
        traveltestContext context = new traveltestContext();

        public User Login(string userName, string pass)
        {
            try
            {
                User account = context.Users.Where(x => x.Email.Trim().ToLower().Equals(userName.Trim().ToLower()) == true && x.Password.Trim().ToLower().Equals(pass.Trim().ToLower()) == true).FirstOrDefault();
                if (account != null)
                {
                    return account;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }
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

        public bool IsPhoneNumberValidVietnam(string phoneNumber)
        {
            // Define a regular expression pattern for Vietnamese phone numbers
            // This pattern assumes the country code is +84 and follows the format 10 digits after that.
            string pattern = @"^0[0-9]{9}$";

            // Use Regex.IsMatch to check if the phone number matches the pattern
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public bool IsEmailValid(string emailAddress)
        {
            // Define a regular expression pattern for a basic email address format
            // This is a simplified pattern and may not cover all edge cases
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            // Use Regex.IsMatch to check if the email address matches the pattern
            return Regex.IsMatch(emailAddress, pattern);
        }
        public bool IsValidFirstnameorLastname(string name)
        {
            // You can add your validation rules here.
            // Example: Check if the firstname is not empty and contains only letters.
            return !string.IsNullOrWhiteSpace(name) && IsAlphaOnly(name);
        }
        public bool IsAlphaOnly(string input)
        {
            // Check if the input string contains only letters.
            foreach (char c in input)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }
        public User getUser(string Email)
        {
            try
            {
                User users = context.Users.Where(x => x.Email == Email).FirstOrDefault();
                if (users != null)
                {
                    return users;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }
        public Boolean ChangePass(User account, string newPass)
        {
            try
            {
                User a = context.Users.Where(x => x.Email == account.Email.Trim() && x.Password == account.Password.Trim()).FirstOrDefault();
                a.Password = newPass;
                context.SaveChanges();
                return true;
            }
            catch
            {
            }
            return false;
        }

        public List<User> GetListUserRegisterTravelAgent()
        {
            List<User> listuserregistertravelagent = new List<User>();
            try
            {
                listuserregistertravelagent = context.Users.Where(x => x.RoleId == 4 || x.RoleId == 2).ToList();
                return listuserregistertravelagent;
            }
            catch
            {
                return listuserregistertravelagent;
            }
        }
        public void AccessRegisterTravelAgent(int id, string stt)
        {
            User a = context.Users.Where(x => x.UserId == id).FirstOrDefault();
            if (stt == "Accept")
            {
                a.RoleId = 2;
                context.SaveChanges();
            }
            else
            {
                a.RoleId = 4;
                context.SaveChanges();
            }
        }
        public void AccessBookingTravel(int id, string stt)
        {
            Booking b = context.Bookings.Where(x => x.BookingId == id).FirstOrDefault();
            if (stt == "Accept")
            {
                b.Status = 4;
                context.SaveChanges();
            }
            else
            {
                b.Status = 3;
                context.SaveChanges();
            }
        }
        public void ResetPass(int id, string email)
        {
            User a = context.Users.Where(x => x.UserId == id).FirstOrDefault();
            a.Password = "Ab@123456";
            context.SaveChanges();
        }
        public List<User> GetListAccount()
        {
            List<User> listaccount = new List<User>();
            try
            {
                listaccount = context.Users.Where(x=>x.RoleId != 1 && x.RoleId !=4).ToList();
                return listaccount;
            }
            catch
            {
                return listaccount;
            }
        }
    }
}