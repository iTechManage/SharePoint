using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace CCSAdvancedAlerts 
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class AdvancedAlertEventReceiver : SPItemEventReceiver
    {


        LoggingManager LogManager = new LoggingManager();
        

       ///// <summary>
       ///// An item is being added.
       ///// </summary>
       //public override void ItemAdding(SPItemEventProperties properties)
       //{
       //    base.ItemAdding(properties);
       //}

       ///// <summary>
       ///// An item is being updated.
       ///// </summary>
       //public override void ItemUpdating(SPItemEventProperties properties)
       //{
       //    base.ItemUpdating(properties);
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
               ExecuteReceivedEvent(ReceivedEventType.ItemAdded, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing itemadded event : " + Ex.Message);
           }
          
       }

       /// <summary>
       /// An item was updated.
       /// </summary>
       public override void ItemUpdated(SPItemEventProperties properties)
       {
           try
           {
               LogManager.write("entered in to ItemUpdated event");
               ExecuteReceivedEvent(ReceivedEventType.ItemUpdated, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing ItemUpdated event : " + Ex.Message);
           }

           
       }

       /// <summary>
       /// An item was deleted.
       /// </summary>
       public override void ItemDeleted(SPItemEventProperties properties)
       {
           try
           {
               LogManager.write("entered in to ItemDeleted event");
               ExecuteReceivedEvent(ReceivedEventType.ItemDeleted, properties);
           }
           catch (System.Exception Ex)
           {
               LogManager.write("error occured whule executing ItemDeleted event : " + Ex.Message);
           }

       }


       private void ExecuteReceivedEvent(ReceivedEventType eventType, SPItemEventProperties properties)
       {
           LogManager.write("Entered in to ExecuteReceivedEvent with event type" + eventType);
           try
           {
               using (SPWeb web = properties.OpenWeb())
               {
                   //TODO we have to check is feature activated for this site or not



               }

           }
           catch (System.Exception Ex)
           {
               LogManager.write("Error occured white excuting event receiver" + Ex.Message);
           }

       }


    }
}
