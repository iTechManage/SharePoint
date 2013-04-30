using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Collections;
using System.Collections.Generic;

namespace CCSAdvancedAlerts 
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class AdvancedAlertEventReceiver : SPItemEventReceiver
    {
        string FinalBody = string.Empty;

        LoggingManager LogManager = new LoggingManager();
        

       ///// <summary>
       ///// An item is being added.
       ///// </summary>
       //public override void ItemAdding(SPItemEventProperties properties)
       //{
       //    base.ItemAdding(properties);
       //}

     

       ///// <summary>
       ///// An item is being deleted.
       ///// </summary>
       //public override void ItemDeleting(SPItemEventProperties properties)
       //{
       //    base.ItemDeleting(properties);
       //}

       /// <summary>
       /// An item was added.
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           try
           {
               LogManager.write("entered in to itemadded event");
               ExecuteReceivedEvent(AlertEventType.ItemAdded, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing itemadded event : " + Ex.Message);
           }
          
       }

       /// <summary>
       /// An item is being updated.
       /// </summary>
       public override void ItemUpdating(SPItemEventProperties properties)
       {
           string body1= "Updated Columns";
           string body = string.Empty;
           string temp = string.Empty;
           string temp2 = string.Empty;
           try
           {
               LogManager.write("entered in to ItemUpdated event");
               foreach (SPField field in properties.ListItem.Fields)
               {
                   if (field != null || field.Hidden)
                   {
                       if (Convert.ToString(properties.ListItem[field.Id]) != Convert.ToString(properties.AfterProperties[field.Title]))
                       {
                           temp = Convert.ToString(properties.AfterProperties[field.Title]);
                           temp2 = Convert.ToString(properties.ListItem[field.Id]);
                           if (!string.IsNullOrEmpty(temp))
                           {
                               if (temp.Equals("0;#"))
                               {
                                   temp = string.Empty;
                               }
                               else if (temp.Contains(";#"))
                               {
                                   temp = temp.Substring(temp.IndexOf(";#") + 2);
                               }

                           }
                           if (!string.IsNullOrEmpty(temp2))
                           {
                               if (temp2.Equals("0;#"))
                               {
                                   temp2 = string.Empty;
                               }
                               else if (temp2.Contains(";#"))
                               {
                                   temp2 = temp2.Substring(temp2.IndexOf(";#") + 2);
                               }

                           }
                           if(!string.IsNullOrEmpty(temp) && !body.Contains(field.Title))
                           {
                           body += "<tr>" + "<td>" + field.Title +"</td>"+ "<td bgcolor='#F0F0F0'>" + "<strike>"+temp2+"</strike>"+"&nbsp;"+"&nbsp;"+"&nbsp;"+temp+"</td>"+"</tr>";
                           }
                       }
                   }
               }
               FinalBody = "<b>"+body1+"</b>" + "<br>" + "<br>" + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + body + "</table>";
               ExecuteReceivedEvent(AlertEventType.ItemUpdated, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing ItemUpdated event : " + Ex.Message);
           }
       }




       /// <summary>
       /// An item was deleted.
       /// </summary>
       public override void ItemDeleting(SPItemEventProperties properties)
       {
           try
           {
               LogManager.write("entered in to ItemDeleted event");
               ExecuteReceivedEvent(AlertEventType.ItemDeleted, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing ItemDeleted event : " + Ex.Message);
           }

       }

       private void ExecuteReceivedEvent(AlertEventType eventType, SPItemEventProperties properties)
       {
           LogManager.write("Entered in to ExecuteReceivedEvent with event type" + eventType);
           try
           {
               
               using (SPWeb web = properties.OpenWeb())
               {
                   //TODO we have to check is feature activated for this site or not
                   AlertManager alertManager = new AlertManager(web.Site.Url);
                   MailTemplateManager mailTemplateManager = new MailTemplateManager(web.Site.Url);
                   IList<Alert> alerts = alertManager.GetAlertForList(properties.ListItem ,eventType, mailTemplateManager);
                   Notifications notifications = new Notifications();
                   foreach (Alert alert in alerts)
                   {
                       if (eventType != AlertEventType.DateColumn)
                       {

                           if (alert.IsValid(properties.ListItem, eventType, properties))
                           {
                               

                               if (alert.SendType == SendType.Immediate)
                               {

                                   notifications.SendMail(alert, eventType, properties.ListItem, FinalBody);
                               }
                               else
                               {
                                   CreateDelayedAlert(alert, eventType, properties, alertManager);
                               }
                           }
                       }
                   }
               }
           }
           catch (System.Exception Ex)
           {
               LogManager.write("Error occured white excuting event receiver" + Ex.Message);
           }

       }


       private void CreateDelayedAlert(Alert alert, AlertEventType eventType, SPItemEventProperties properties, AlertManager alertManager)
       {
           //, SPWeb web
           try
           {
               //Need to get the Alert instances
               MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(eventType);
               string subject = mtObject.Template.Subject;
               string body = mtObject.Template.Body+"<br>"+"<br>"+FinalBody;
               string parentItemId = Convert.ToString(properties.ListItem.ID);
               DelayedAlert dAlert = new DelayedAlert(subject, body, alert.Id, parentItemId, eventType);
               alertManager.AddDelayedAlert(dAlert);
           }
           catch { }
       }


    }
}
