using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace CCSAdvancedAlerts
{
    class Common
    {

        internal static bool SendMail(string SmtpServer, string To, string From, string CC, string Subject, string Body, List<Attachment> Attachments)
        {
            bool succes = false;
            try
            {
                SmtpClient smtp = new SmtpClient(SmtpServer);
                Utilities.LogManager.write("smtp client created ");
                
                MailMessage msg = new MailMessage();
                msg.IsBodyHtml = true;
                msg.To.Add(To);
                msg.From = new MailAddress(From);
                if (!string.IsNullOrEmpty(CC))
                {
                    msg.CC.Add(CC);
                }
                if (!string.IsNullOrEmpty(Subject))
                {
                    msg.Subject = Subject;
                }
                if (!string.IsNullOrEmpty(Body))
                {
                    msg.Body = Body;
                }
                if (Attachments != null)
                {
                    if (Attachments.Count > 0)
                    {
                        foreach (Attachment attach in Attachments)
                        {
                            msg.Attachments.Add(attach);
                        }
                    }
                }

                smtp.Send(msg);
                succes = true;
            }
            catch (System.Exception ex)
            {
                succes = false;
            }
            return succes;

        }







    }
}
