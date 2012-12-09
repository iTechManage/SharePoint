using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Utilities;

namespace CCSAdvancedAlerts
{
    class Notifications
    {
        public const string ValueCollectionSeperator = ";";
        public const char EmailCollectionSeperator = ';';
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        string PlaceHoldersExpressionPattern = @"\[(.*?)\]";


        internal static bool SendMail(string SmtpServer, string To, string From, string CC, string Subject, string Body, List<Attachment> Attachments)
        {
            bool succes = false;
            try
            {
                if (string.IsNullOrEmpty(To) && string.IsNullOrEmpty(CC))
                    return false;

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
            catch
            {
                succes = false;
            }
            return succes;
        }

        internal bool SendMail(Alert alert, AlertEventType eventType, SPListItem item)
        {
            bool succes = true;
            try
            {
                MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(eventType);

                string toAddress = GetRecipientEmailAddresses(alert.ToAddress, item);
                string ccAddress = GetRecipientEmailAddresses(alert.CcAddress, item);
                string fromAddress = GetRecipientEmailAddresses(alert.FromAdderss, item);

                string subject = ReplacePlaceHolders(mtObject.Template.Subject, item);
                string body = ReplacePlaceHolders(mtObject.Template.Body, item);

                SendMail("ITECHDC",
                         toAddress,
                         fromAddress,
                         ccAddress,
                         subject,
                         body,
                         null);
            }
            catch { succes = false; }

            return succes;

        }

        string GetRecipientEmailAddresses(string addresses, SPListItem item)
        {
            string emailAddresses = string.Empty;
            if (!string.IsNullOrEmpty(addresses))
            {
                foreach (string address in addresses.Split(EmailCollectionSeperator))
                {
                    string email = "";
                    if (isValidEmailAddress(address))
                    {
                        email = address;
                    }
                    else
                    {
                        email = GetEmailAddressFromField(item, address);

                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        if (string.IsNullOrEmpty(emailAddresses))
                        {
                            emailAddresses = email;
                        }
                        else
                        {
                            emailAddresses += ", " + email;
                        }
                    }

                }
            }
            return emailAddresses;
        }

        bool isEmailAddress(string address)
        {
            return address.Contains("@");
        }

        bool isValidEmailAddress(string address)
        {
            if (isEmailAddress(address))
            {
                if (address != null)
                { return Regex.IsMatch(address, MatchEmailPattern); }

                else { return false; }
            }
            return false;
        }

        string GetEmailAddressFromField(SPListItem listItem, string fieldName)
        {
            string strDiplayName = string.Empty;
            string Email = string.Empty;
            SPField field = listItem.Fields.TryGetFieldByStaticName(fieldName);
            if (field != null)
            {
                object fieldValue = listItem[fieldName];
                string strValue = GetFieldValue(fieldValue, field.FieldValueType);
                SPUtility.GetFullNameandEmailfromLogin(listItem.ParentList.ParentWeb, strValue, out strDiplayName, out Email);
            }
            return Email;
        }

        /// <summary>
        /// Replace the placeholders with its original values
        /// </summary>
        /// <param name="template"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        string ReplacePlaceHolders(string template, SPListItem listItem)
        {
            string afterTemplate = string.Empty;
            try
            {
                Regex re = new Regex(PlaceHoldersExpressionPattern);
                foreach (Match match in re.Matches(template))
                {
                    string placeHolder = match.Value.Replace("[", string.Empty).Replace("]", string.Empty);
                    SPField field = listItem.Fields.TryGetFieldByStaticName(placeHolder);
                    if (field != null)
                    {
                        object fieldValue = listItem[placeHolder];
                        string strValue = GetFieldValue(fieldValue, field.FieldValueType);
                        template = template.Replace(match.Value, strValue);
                    }
                }
            }
            catch
            {
            }
            return template;
        }

        /// <summary>
        /// Pass the field value and value type to get the value in string format
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="fieldValueType"></param>
        /// <returns></returns>
        public string GetFieldValue(object fieldValue, Type fieldValueType)
        {

            if (fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString()))
            {
                if (fieldValueType == (typeof(SPFieldUrlValue)))
                {
                    SPFieldUrlValue fieldUrlValue = new SPFieldUrlValue(fieldValue.ToString());
                    return fieldUrlValue.Url;

                }
                else if (fieldValueType == (typeof(SPFieldUserValue)))
                {
                    SPFieldUserValue fieldUserValue = new SPFieldUserValue(SPContext.Current.Web, fieldValue.ToString());

                    string userLoginName = fieldUserValue.User.LoginName;
                    string userDispalyName = fieldUserValue.User.Name;

                    return userLoginName;

                }
                else if (fieldValueType == (typeof(SPFieldUserValueCollection)))
                {
                    SPFieldUserValueCollection fieldUserValueCollection = new SPFieldUserValueCollection(SPContext.Current.Web, fieldValue.ToString());
                    string userLoginNames = "";
                    string userDispalyNames = "";

                    foreach (SPFieldUserValue userValue in fieldUserValueCollection)
                    {
                        userLoginNames += userValue.LookupValue + ValueCollectionSeperator;
                        if (userValue.User != null)
                            userDispalyNames += userValue.User.Name + ValueCollectionSeperator;
                    }

                    userLoginNames = userLoginNames.TrimEnd(ValueCollectionSeperator.ToCharArray());
                    userDispalyNames = userDispalyNames.TrimEnd(ValueCollectionSeperator.ToCharArray());
                    return userDispalyNames;

                }
                else if (fieldValueType == (typeof(SPFieldLookupValue)))
                {
                    SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue(fieldValue.ToString());
                    string strFieldValue = fieldLookupValue.LookupValue;
                    return strFieldValue;
                }
                else if (fieldValueType == (typeof(SPFieldLookupValueCollection)))
                {
                    SPFieldLookupValueCollection fieldLookupValueCollection = new SPFieldLookupValueCollection(fieldValue.ToString());
                    string strFieldValue = string.Empty;
                    foreach (SPFieldLookupValue lookup in fieldLookupValueCollection)
                    {
                        strFieldValue += lookup.LookupValue + ValueCollectionSeperator;
                    }
                    strFieldValue = strFieldValue.TrimEnd(ValueCollectionSeperator.ToCharArray());
                    return strFieldValue;
                }
                else if (fieldValueType == (typeof(DateTime)))
                {
                    DateTime sourceDT = DateTime.Parse(fieldValue.ToString());
                    return sourceDT.ToString();
                }
                else // default matching will be performed with string type
                {
                    return fieldValue.ToString();

                }
            }
            else
            {
                return "";
            }

        }



    }
}
