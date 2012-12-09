using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts
{
    class MailTemplateManager
    {
        private SPList mailTemplateList;
        private SPList mailTemlateUsageList;
        //private string rootSiteURL;

        #region Constructors
        public MailTemplateManager(SPWeb web)
        {
            CheckForExistanceOfMailTemplateList(web);
            CheckForExistanceOfMailTemplateUsageList(web);
        }


        public MailTemplateManager(string siteCollectionURL)
        {
            using (SPSite site = new SPSite(siteCollectionURL))
            {
                using (SPWeb web = site.RootWeb)
                {
                    CheckForExistanceOfMailTemplateList(web);
                    CheckForExistanceOfMailTemplateUsageList(web);
                }
            }
        }


        #endregion Constructors

        public void CheckForExistanceOfMailTemplateList(SPWeb web)
        {
            try
            {
                //Get the MailTemplate list from web if exists
                if (web != null)
                {
                    mailTemplateList = web.Lists.TryGetList(ListAndFieldNames.MTListName);
                    if (mailTemplateList == null)
                    {
                        //Create new list if not exists
                    }
                }
            }
            catch
            { }
        }
        public void CheckForExistanceOfMailTemplateUsageList(SPWeb web)
        {
            try
            {
                //get the MailTemplate Usage list if exists
                if (web != null)
                {
                    mailTemlateUsageList = web.Lists.TryGetList(ListAndFieldNames.MTUListName);
                    if (mailTemlateUsageList == null)
                    {
                        //Create new list if not exist
                    }
                }
            }
            catch
            { }
        }


        public void AddTemplate(MailTemplate template)
        {
            try
            {
                //SPList mailTemplateList = SPContext.Current.Site.RootWeb.Lists.TryGetList(ListAndFieldNames.MTListName);

                if (mailTemplateList != null)
                {
                    SPListItem listItem = mailTemplateList.AddItem();
                    listItem["Title"] = template.Name;
                    listItem[ListAndFieldNames.MTListMailSubjectFieldName] = template.Subject;
                    listItem[ListAndFieldNames.MTListMailBodyFieldName] = template.Body;
                    listItem[ListAndFieldNames.MTListInsertUpdatedFieldsFieldName] = template.InsertUpdatedFields;
                    listItem[ListAndFieldNames.MTListInsertAttachmentsFieldName] = template.InsertAttachments;
                    listItem[ListAndFieldNames.MTListHighLightUpdatedFieldsFieldName] = template.HighLightUpdatedFields;
                    //listItem[ListAndFieldNames.MTListOwnerFieldName] = template.;


                    listItem.Update();
                }

            }
            catch { }
        }

        public void AddMailTemplateUsageObject(string alertID, MailTemplateUsageObject mObject)
        {
            try
            {

                if(mailTemlateUsageList != null)
                {
                        SPListItem listItem = mailTemlateUsageList.AddItem();
                        listItem[ListAndFieldNames.MTUAlertFieldName] = alertID + ";#" +  alertID;
                        listItem[ListAndFieldNames.MTUTemplateFieldName] = mObject.Template.ID + ";#" + mObject.Template.Name;

                        //Event Type Registered
                        foreach (AlertEventType aType in mObject.AlertType)
                        {
                            listItem[ListAndFieldNames.settingsListEventTypeFieldName] += aType + ";#";
                        }


                        listItem[ListAndFieldNames.MTUHighLightUpdatedFieldsFieldName] = mObject.HighLightUpdatedFields;
                        listItem[ListAndFieldNames.MTUInsertAttachmentsFieldName] = mObject.InsertAttachments;
                        listItem[ListAndFieldNames.MTUInsertUpdatedFieldsFieldName] = mObject.InsertUpdatedFields;

                        //Other information in xml format
                        listItem.Update();

                }
                else
                {
                    //unable to get mailtemplate usage list
                }

            }
            catch { }
        }


        internal   MailTemplate GetMailTemplateFromListItem(SPListItem listItem)
        {
            MailTemplate mTempalte = new MailTemplate();

            mTempalte.Name = Convert.ToString(listItem["Title"]);
            mTempalte.Subject =Convert.ToString( listItem[ListAndFieldNames.MTListMailSubjectFieldName]);
            mTempalte.Body = Convert.ToString(listItem[ListAndFieldNames.MTListMailBodyFieldName]);
            mTempalte.InsertUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTListInsertUpdatedFieldsFieldName]);
            mTempalte.InsertAttachments = Convert.ToBoolean(listItem[ListAndFieldNames.MTListInsertAttachmentsFieldName]);
            mTempalte.HighLightUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTListHighLightUpdatedFieldsFieldName]);

            return mTempalte;
        }


        internal  MailTemplateUsageObject GetMailTemplateUsageObjectFromListItem(SPListItem listItem)
        {

            MailTemplateUsageObject mObject = new MailTemplateUsageObject();

           // listItem[ListAndFieldNames.settingsListListIdFieldName] 
            //get the template for this instance
            //string strTemplate = Convert.ToString(listItem[ListAndFieldNames.settingsListEventTypeFieldName]);
            
            SPFieldLookupValue lookupTempalte = new SPFieldLookupValue(listItem[ListAndFieldNames.MTUTemplateFieldName].ToString());
            SPListItem tListItem = mailTemplateList.GetItemById(lookupTempalte.LookupId);
            mObject.Template = GetMailTemplateFromListItem(tListItem);

            //Event Type Registered
            string strEventType = Convert.ToString(listItem[ListAndFieldNames.MTUEventTypeFieldName]);
            if (strEventType.Contains(AlertEventType.ItemAdded.ToString()))
            {
                mObject.AlertType.Add(AlertEventType.ItemAdded);
            }
            if (strEventType.Contains(AlertEventType.ItemDeleted.ToString()))
            {
                mObject.AlertType.Add(AlertEventType.ItemDeleted);
            }
            if (strEventType.Contains(AlertEventType.ItemUpdated.ToString()))
            {
                mObject.AlertType.Add(AlertEventType.ItemUpdated);
            }
            if (strEventType.Contains(AlertEventType.DateColumn.ToString()))
            {
                mObject.AlertType.Add(AlertEventType.DateColumn);
            }

            mObject.HighLightUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTUHighLightUpdatedFieldsFieldName]);
            mObject.InsertAttachments = Convert.ToBoolean(listItem[ListAndFieldNames.MTUInsertAttachmentsFieldName]);
            mObject.InsertUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTUInsertUpdatedFieldsFieldName]);

            return mObject;
        }


        internal MailTemplateUsageObject GetTemplateUsageObjectForAlert(string alertId, AlertEventType eventType)
        {
            MailTemplateUsageObject mObject = null;
            try
            {
                SPQuery query = new SPQuery();
                string strQuery = string.Format("<Where><And><Eq><FieldRef Name=\"{0}\"/><Value Type=\"Choice\">{1}</Value></Eq><Eq><FieldRef Name=\"Alert\" LookupId=\"TRUE\"/><Value Type=\"Lookup\">{2}</Value></Eq></And></Where>", "ChangeTypes", eventType.ToString(), alertId);
                SPListItemCollection listItemcollection = mailTemlateUsageList.GetItems(query);
                if (listItemcollection != null && listItemcollection.Count > 0)
                {
                    mObject = GetMailTemplateUsageObjectFromListItem(listItemcollection[0]);
                }


            }
            catch{}

            return mObject;
        }


        internal List<MailTemplateUsageObject> GetTemplateUsageObjects(string alertID)
        {
            List<MailTemplateUsageObject> mtuObjects = new List<MailTemplateUsageObject>();
            try
            {
                if (mailTemlateUsageList != null)
                {
                    //get all the usageObjects for the alert
                    SPQuery query = new SPQuery();
                    string strQuery = string.Format("<Where><Eq><FieldRef Name=\"Alert\" LookupId=\"TRUE\"/><Value Type=\"Lookup\">{0}</Value></Eq></Where>", alertID);
                    query.Query = strQuery;
                    SPListItemCollection itemcollection =    mailTemlateUsageList.GetItems(query);
                    foreach (SPListItem item in itemcollection)
                    {
                        mtuObjects.Add(GetMailTemplateUsageObjectFromListItem(item));
                    }
                }
            }
            catch { }
            return mtuObjects;
        }
    }
}
