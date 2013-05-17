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

        static string SMTPServerName;



        public string siteCollectionURL;
        internal void SendDelayedMessage(DelayedAlert delayedAlert, Alert alert, SPListItem item)
        {
            try
            {               
                MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(delayedAlert.AlertType);
                string toAddress = GetRecipientEmailAddresses(alert.ToAddress, item);
                string ccAddress = GetRecipientEmailAddresses(alert.CcAddress, item);
                string fromAddress = GetRecipientEmailAddresses(alert.FromAdderss, item);
                string subject = delayedAlert.Subject;
                string body = delayedAlert.Body;
                //string subject = ReplacePlaceHolders(mtObject.Template.Subject, item);
                //string body = ReplacePlaceHolders(mtObject.Template.Body, item);
                string smtpSName = GetSMTPServer(item);
                SendMail(smtpSName,
                         toAddress,
                         fromAddress,
                         ccAddress,
                         subject,
                         body,
                         null);
            }
            catch { }
            //try
            //{
            //    SPListItem item = null;
            //    using (SPSite site = new SPSite(this.siteCollectionURL))
            //    {
            //        site.CatchAccessDeniedException = false;
            //        try
            //        {
            //            using (SPWeb web = site.OpenWeb(alert.WebId))
            //            {
            //               // item = ScanningUtilities.GetItemFromList(web, alert.ListId, delayedAlert.Item.ID);
            //                //if (item == null)
            //                {
            //                    item = delayedAlert.Item;
            //                }

            //            }
            //        }
            //        catch 
            //        {
            //            item = delayedAlert.Item;
            //        }

            //    }
            //}
            //catch 
            //{
            //}
        }
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

        internal bool SendMail(Alert alert, AlertEventType eventType, SPListItem item, string strAfterProperties)
        {
            bool succes = true;
            string body = string.Empty;
            try
            {
                MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(eventType);
                List<Attachment> attachmentsToSend = null;

                if (mtObject.Template.InsertAttachments)
                {
                    if (item.Attachments != null && item.Attachments.Count > 0)
                    {
                        if (attachmentsToSend == null)
                        {
                            attachmentsToSend = new List<Attachment>();
                        }

                        foreach (string fileName in item.Attachments)
                        {
                            SPFile file = item.ParentList.ParentWeb.GetFile(item.Attachments.UrlPrefix + fileName);
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(file.OpenBinaryStream(), fileName, string.Empty);
                            attachmentsToSend.Add(attachment);
                        }
                    }
                }
                string toAddress = GetRecipientEmailAddresses(alert.ToAddress, item);
                string ccAddress = GetRecipientEmailAddresses(alert.CcAddress, item);
                string fromAddress = GetRecipientEmailAddresses(alert.FromAdderss, item);
                string subject = ReplacePlaceHolders(mtObject.Template.Subject, item);
                if (mtObject.Template.InsertUpdatedFields)
                {
                     body = ReplacePlaceHolders(mtObject.Template.Body, item) + "<br>" + "<br>" + strAfterProperties;
                }
                else
                {
                     body = ReplacePlaceHolders(mtObject.Template.Body, item);
                }

                string smtpSName = GetSMTPServer(item);
                SendMail(smtpSName,
                         toAddress,
                         fromAddress,
                         ccAddress,
                         subject,
                         body,
                         attachmentsToSend);
            }
            catch { succes = false; }
            return succes;
        }

        internal bool SendMail(Alert alert, AlertEventType eventType, SPListItem item)
        {
            //bool succes = true;
            //try
            //{
            //    MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(eventType);

            //    string toAddress = GetRecipientEmailAddresses(alert.ToAddress, item);
            //    string ccAddress = GetRecipientEmailAddresses(alert.CcAddress, item);
            //    string fromAddress = GetRecipientEmailAddresses(alert.FromAdderss, item);

            //    string subject = ReplacePlaceHolders(mtObject.Template.Subject, item);
            //    string body = ReplacePlaceHolders(mtObject.Template.Body, item);

            //    string smtpSName = GetSMTPServer(item);

            //    SendMail(smtpSName,
            //             toAddress,
            //             fromAddress,
            //             ccAddress,
            //             subject,
            //             body,
            //             null);
            //}
            //catch { succes = false; }

            //return succes;
            return SendMail(alert, eventType, item, string.Empty);
        }
        public string GetRecipientEmailAddresses(string addresses, SPListItem item)
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

       public string GetEmailAddressFromField(SPListItem listItem, string fieldName)
        {
            //string strDiplayName = string.Empty;
            //string Email = string.Empty;
            //SPField field = listItem.Fields.TryGetFieldByStaticName(fieldName);
            //if (field != null)
            //{
            //    object fieldValue = listItem[fieldName];
            //    string strValue = GetFieldValue(fieldValue, field.FieldValueType);
            //    SPUtility.GetFullNameandEmailfromLogin(listItem.ParentList.ParentWeb, strValue, out strDiplayName, out Email);
            //}
            //return Email;

            string strDiplayName = string.Empty;
            string Email = string.Empty;
            if (!string.IsNullOrEmpty(fieldName)) //&& fieldName.IndexOf(',') != -1
            {
                string[] fieldNames = fieldName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string strFieldName in fieldNames)
                {
                    string strCurrentEmail = string.Empty;
                    //SPField field = listItem.Fields.TryGetFieldByStaticName(strFieldName);
                    SPField field = null;
                    try
                    {
                        field = listItem.Fields.GetField(strFieldName);
                    }
                    catch { }
                    if (field != null)
                    {
                        object fieldValue = listItem[strFieldName];
                        strCurrentEmail = this.GetUserEmailFromField(Convert.ToString(fieldValue), field);

                        if (!string.IsNullOrEmpty(strCurrentEmail))
                        {
                            if (!string.IsNullOrEmpty(Email))
                            {
                                Email += ",";
                            }
                            Email += strCurrentEmail;
                        }
                    }
                }
            }
            return Email;
        }

        /// <summary>
        /// Replace the placeholders with its original values
        /// </summary>
        /// <param name="template"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
       public string ReplacePlaceHolders(string template, SPListItem listItem)
        {
            string afterTemplate = string.Empty;
            try
            {
                Regex re = new Regex(PlaceHoldersExpressionPattern);
                foreach (Match match in re.Matches(template))
                {
                    string placeHolder = match.Value.Replace("[", string.Empty).Replace("]", string.Empty);
                    if (placeHolder.Equals("Item Link"))
                    {
                        string linkToTicket = string.Empty;
                        try
                        {
                            string currentURL = listItem.ParentList.ParentWeb.Site.MakeFullUrl(listItem.ParentList.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl) +
                                "?ID=" + listItem.ID + "&Source=" + listItem.ParentList.ParentWeb.Site.MakeFullUrl(listItem.ParentList.ParentWeb.ServerRelativeUrl);

                            if (!string.IsNullOrEmpty(currentURL))
                            {
                                SPSecurity.RunWithElevatedPrivileges(delegate
                                {
                                    string name = listItem.Title;
                                
                                linkToTicket = "<a href=\"" + currentURL + "\">"+name+"</a>"+"<br>";
                                    });
                            }
                        }

                        catch (System.Exception ex)
                        {

                        }
                        template = template.Replace(match.Value, linkToTicket);

                    }
                    else if (placeHolder.Equals("Item Title"))
                    {
                        string itemTitle = listItem.Title+"<br>";
                        
                        template = template.Replace(match.Value, itemTitle);

                    }
                    else if (placeHolder.Equals("Site Link"))
                    {
                        string linkToSite = string.Empty;
                        try
                        {
                            string currentURL = listItem.ParentList.ParentWeb.Site.Url;

                            if (!string.IsNullOrEmpty(currentURL))
                            {
                                SPSecurity.RunWithElevatedPrivileges(delegate
                                {
                                    string name = listItem.ParentList.ParentWeb.Site.RootWeb.Title;

                                    linkToSite = "<a href=\"" + currentURL + "\">" + name + "</a>"+"<br>";
                                });
                            }
                        }

                        catch (System.Exception ex)
                        {

                        }
                        template = template.Replace(match.Value, linkToSite);
                    }
                    else if (placeHolder.Equals("Site Title"))
                    {
                        string siteTitle = listItem.ParentList.ParentWeb.Site.RootWeb.Title+"<br>";

                        template = template.Replace(match.Value, siteTitle);

                    }
                    else if (placeHolder.Equals("List Link"))
                    {
                        string linkToList = string.Empty;
                        try
                        {
                            string currentURL = listItem.ParentList.DefaultViewUrl;

                            if (!string.IsNullOrEmpty(currentURL))
                            {
                                SPSecurity.RunWithElevatedPrivileges(delegate
                                {
                                    string name = listItem.ParentList.Title;

                                    linkToList = "<a href=\"" + currentURL + "\">" + name + "</a>"+"<br>";
                                });
                            }
                        }

                        catch (System.Exception ex)
                        {

                        }
                        template = template.Replace(match.Value, linkToList);
                    }
                    else if (placeHolder.Equals("List Title"))
                    {
                        string listTitle = listItem.ParentList.Title+"<br>";

                        template = template.Replace(match.Value, listTitle);

                    }
                    else if (placeHolder.Equals("Edit Item"))
                    {
                        string editItem = string.Empty;
                        try
                        {
                            string currentURL = listItem.ParentList.ParentWeb.Site.MakeFullUrl(listItem.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl) +
                                "?ID=" + listItem.ID + "&Source=" + listItem.ParentList.ParentWeb.Site.MakeFullUrl(listItem.ParentList.ParentWeb.ServerRelativeUrl);

                            if (!string.IsNullOrEmpty(currentURL))
                            {
                                SPSecurity.RunWithElevatedPrivileges(delegate
                                {
                                    editItem = "<a href=\"" + currentURL + "\">Edit Item</a>"+"<br>";
                                });
                            }
                        }

                        catch (System.Exception ex)
                        {

                        }
                        template = template.Replace(match.Value, editItem);
                    }
                    else if (placeHolder.Equals("Edit Alerts"))
                    {
                        string editAlert = string.Empty;
                        try
                        {
                            string currentURL = listItem.ParentList.ParentWeb.Site.Url +
                                "/_layouts/CCSAdvancedAlerts/AdvancedAlertSettings.aspx?" + "Source=" + listItem.ParentList.ParentWeb.Site.Url+"/SitePages/Home.aspx";

                            if (!string.IsNullOrEmpty(currentURL))
                            {
                                SPSecurity.RunWithElevatedPrivileges(delegate
                                {
                                    editAlert = "<a href=\"" + currentURL + "\">Edit Alerts</a>"+"<br>";
                                });
                            }
                        }

                        catch (System.Exception ex)
                        {

                        }
                        template = template.Replace(match.Value, editAlert);
                    }
                    else
                    {
                        SPField field = null;
                        try
                        {
                            field = listItem.Fields[placeHolder];
                        }
                        catch { }
                        if (field != null)
                        {
                            object fieldValue = listItem[placeHolder];
                            string strValue = GetFieldValue(fieldValue, field.FieldValueType, listItem.ParentList.ParentWeb);
                            template = template.Replace(match.Value, strValue);
                        }
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
        internal string GetFieldValue(object fieldValue, Type fieldValueType, SPWeb web)
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
                    SPFieldUserValue fieldUserValue = new SPFieldUserValue(web, fieldValue.ToString());

                    string userLoginName = fieldUserValue.User.LoginName;
                    string userDispalyName = fieldUserValue.User.Name;

                    return userDispalyName;

                }
                else if (fieldValueType == (typeof(SPFieldUserValueCollection)))
                {
                    SPFieldUserValueCollection fieldUserValueCollection = new SPFieldUserValueCollection(web, fieldValue.ToString());
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

        string GetUserEmailFromField(string strUserFieldValue, SPField personOrGroupField)
        {
            string emailAddressToReturn = string.Empty;

            if (!string.IsNullOrEmpty(strUserFieldValue) && strUserFieldValue.Contains(";#"))
            {
                if (personOrGroupField != null &&
                    personOrGroupField.Type == SPFieldType.User)
                {
                    try
                    {
                        SPFieldUser userField = (SPFieldUser)personOrGroupField;

                        if (userField != null)
                        {
                            if (userField.AllowMultipleValues)
                            {
                                SPFieldUserValueCollection userFieldValueColl = (SPFieldUserValueCollection)userField.GetFieldValue(strUserFieldValue);

                                if (userFieldValueColl != null &&
                                    userFieldValueColl.Count > 0)
                                {
                                    foreach (SPFieldUserValue userValue in userFieldValueColl)
                                    {
                                        SPUser spUser = userValue.User;

                                        if (spUser != null &&
                                            !string.IsNullOrEmpty(spUser.Email))
                                        {
                                            if (!string.IsNullOrEmpty(emailAddressToReturn))
                                            {
                                                if (!emailAddressToReturn.Contains(spUser.Email))
                                                {
                                                    emailAddressToReturn = emailAddressToReturn + "," + spUser.Email;
                                                }
                                            }
                                            else
                                            {
                                                emailAddressToReturn = spUser.Email;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SPFieldUserValue userFieldValue = (SPFieldUserValue)userField.GetFieldValue(strUserFieldValue);

                                if (userFieldValue != null)
                                {
                                    SPUser spUser = userFieldValue.User;

                                    if (spUser != null &&
                                        !string.IsNullOrEmpty(spUser.Email))
                                    {
                                        emailAddressToReturn = spUser.Email;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            return emailAddressToReturn;
        }

       public string GetSMTPServer(SPListItem lItem)
        {
            if (string.IsNullOrEmpty(SMTPServerName))
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        SMTPServerName = lItem.ParentList.ParentWeb.Site.WebApplication.OutboundMailServiceInstance.Server.Address;
                    });
                }
                catch
                { }
            }
            return SMTPServerName;
        }

    }
}
