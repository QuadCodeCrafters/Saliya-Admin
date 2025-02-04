using CredentialManagement;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ModernApp.Core
{
    public class MailSender
    {
        // Fetch credentials securely from Windows Credential Manager
        public static string ApiKey => CredentialManager.GetCredential("SaliyaAutoCare/apiKey")?.Password;
        public static string ApiSecret => CredentialManager.GetCredential("SaliyaAutoCare/apiSecret")?.Password;
        public static string SenderEmail => CredentialManager.GetCredential("SaliyaAutoCare/Email")?.Password;

        public static async Task<bool> SendEmailAsync(string recipientEmail, string recipientName)
        {
            try
            {
                MailjetClient client = new MailjetClient(ApiKey, ApiSecret);
                var request = new MailjetRequest { Resource = SendV31.Resource }
                    .Property(Send.Messages, new JArray {
                        new JObject {
                            { "From", new JObject { { "Email", SenderEmail }, { "Name", "Saliya Auto Care HR" } } },
                            { "To", new JArray {
                                new JObject { { "Email", recipientEmail }, { "Name", recipientName } }
                            }},
                            { "Subject", "⚠️ Attendance Alert: 3 Consecutive Days Absence" },
                            { "TextPart", $"Dear {recipientName},\n\nWe noticed that you have been absent for 3 consecutive days. Please contact HR immediately.\n\nBest regards,\nSaliya Auto Care HR" },
                            { "HTMLPart", $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                                    <div style='text-align: center; padding: 15px; background-color: #333; color: #fff; border-radius: 10px 10px 0 0;'>
                                        <h2 style='margin: 0;'>Saliya Auto Care</h2>
                                        <p style='margin: 0;'>Your Trusted Auto Service Partner</p>
                                    </div>
                                    <div style='padding: 20px; background-color: #fff;'>
                                        <h3 style='color: #333;'>Dear {recipientName},</h3>
                                        <p style='font-size: 16px; color: #555;'>We have noticed that you have been absent for <b>three consecutive days</b>. Please contact the HR department as soon as possible to clarify your situation.</p>
                                        <p style='font-size: 14px; color: #777;'>If you need any assistance or have a valid reason, feel free to reach out.</p>
                                        <br>
                                        <p style='text-align: center;'>
                                            <a href='mailto:hr@saliyaautocare.com' style='background-color: #d9534f; color: #fff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Contact HR</a>
                                        </p>
                                    </div>
                                    <div style='text-align: center; padding: 10px; background-color: #333; color: #fff; border-radius: 0 0 10px 10px; font-size: 12px;'>
                                        <p>&copy; 2025 Saliya Auto Care. All rights reserved.</p>
                                    </div>
                                </div>
                            "}
                        }
                    });

                var response = await client.PostAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                return false;
            }
        }
    }

    public class CredentialManager
    {
        public static NetworkCredential GetCredential(string target)
        {
            var cred = new Credential { Target = target };
            return cred.Load() ? new NetworkCredential(cred.Username, cred.Password) : null;
        }
    }
}
