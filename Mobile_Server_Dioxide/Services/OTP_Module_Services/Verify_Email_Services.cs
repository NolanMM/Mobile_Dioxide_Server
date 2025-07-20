using System.Net.Mail;
using System.Net;

namespace Mobile_Server_Dioxide.Services.OTP_Module_Services
{
    public class Verify_Email_Services
    {
        private static readonly Random random = new Random();
        private const string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static string _almanac_logo_small = "https://drive.usercontent.google.com/download?id=1X9B5QozoZfv8k4BKhmLVen9H-U-zLfcx&authuser=0";

        private static readonly string from_email = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? "capstonedioxieteam@gmail.com";
        private static readonly string password_smtp = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";

        public static string GenerateRandomKey(int length)
        {
            return new string(Enumerable.Repeat(allowedCharacters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static public async Task<bool> Send_OTP_CodeAsync(string OTP_CODE, string OTP_CODE_ID, string email_to, string username)
        {
            String messageBody;//from, pass,

            string endpointLink = $"https://localhost:7027/api/mobiledioxie/Register_User/{OTP_CODE}/{OTP_CODE_ID}";

            messageBody = CreateEmailBody(OTP_CODE, endpointLink, username);

            MailMessage email = new MailMessage();
            email.From = new MailAddress(from_email, "Dioxie Team | Conestoga");
            email.To.Add(email_to);
            email.Body = messageBody;
            email.Subject = "OTP Verify Code";
            email.IsBodyHtml = true; // Enable HTML formatting
            email.Priority = MailPriority.High;
            email.Headers.Add("X-Priority", "1");

            // Generate smtp server to send the verify email 
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.EnableSsl = true;
            SmtpServer.Port = 587;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            // Checking the app password of the google email and the email of the sender
            SmtpServer.Credentials = new NetworkCredential(from_email, password_smtp);

            try
            {
                await SmtpServer.SendMailAsync(email);
                // Console.WriteLine("Email Successfully Sent");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        private static string CreateEmailBody(string OTP_CODE, string endpointLink, string username)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      <base target='_top'>
                    </head>
                    <body>
                      <div style='font-family: Helvetica, Arial, sans-serif; min-width: 1000px; overflow: auto; line-height: 2'>
                        <div style='margin: 50px auto; width: 80%; padding: 20px 0'>
                            <div style='border-bottom: 5px solid #eee'>
                                <a href='' style='font-size: 30px; color: #CC0000; text-decoration: none; font-weight: 600'>   
                                    Almanac Capstone Project | Dioxie Team
                                </a>
                            </div>
                            <br>
                        <p style='font-size: 22px'>Hello {username},</p>
                        <p>Thank you for choosing our services. Please use this OTP to complete your Sign Up procedures and verify your account.</p>
                        <p>Remember, Never share this OTP with anyone.</p><br><br>
                          <h2 style='margin: 0 auto; width: max-content; padding: 0 10px; color: #CC0000; border-radius: 4px;'>{OTP_CODE}</h2><br>
                          <div style='text-align: center;'>
                          <a href='{endpointLink}' style='background: #CC0000; color: #fff; text-decoration: none; padding: 2px 16px; border-radius: 10px; display: inline-block;'>Verify OTP</a><br><br></div><br>
                          <p style='font-size: 15px;'>Regards,<br /><br>Dioxie Team | Conestoga</p>
                          <hr style='border: none; border-top: 5px solid #eee' />
                          <div style='float: left; padding: 8px 0; color: #aaa; font-size: 0.8em; line-height: 1; font-weight: 300'>
                           <img src='{_almanac_logo_small}' alt='NolanM Logo' style='width: 50%; max-height: 108px;'>
                        </div>
                        </div>
                      </div>
                    </body>
                    </html>
                    ";
        }
    }
}
