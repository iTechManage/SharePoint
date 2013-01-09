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

        #region CheckForExistanceOfLists

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

        #endregion

        #region Template Related

        public void AddTemplate(MailTemplate template)
        {
            try
            {
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

        internal MailTemplate GetMailTemplateFromListItem(SPListItem listItem)
        {
            MailTemplate mTempalte = new MailTemplate();

            mTempalte.Name = Convert.ToString(listItem["Title"]);
            mTempalte.ID = Convert.ToString(listItem.ID);
            mTempalte.Subject = Convert.ToString(listItem[ListAndFieldNames.MTListMailSubjectFieldName]);
            mTempalte.Body = Convert.ToString(listItem[ListAndFieldNames.MTListMailBodyFieldName]);
            mTempalte.InsertUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTListInsertUpdatedFieldsFieldName]);
            mTempalte.InsertAttachments = Convert.ToBoolean(listItem[ListAndFieldNames.MTListInsertAttachmentsFieldName]);
            mTempalte.HighLightUpdatedFields = Convert.ToBoolean(listItem[ListAndFieldNames.MTListHighLightUpdatedFieldsFieldName]);

            return mTempalte;
        }

        internal Dictionary<string, string> GetTemplatesByUser(int userID)
        {
            Dictionary<string, string> templatesByUser = new Dictionary<string, string>();
            try
            {
                //Iterate througu all the alerts for the owners
                foreach (SPListItem item in mailTemplateList.Items)
                {
                    //Push them to Dict
                    if (item["Owner"] != null)
                    {
                        SPUser user = new SPFieldUserValue(SPContext.Current.Web, item["Owner"].ToString()).User;
                        if (user.ID == userID)
                        {
                            templatesByUser.Add(Convert.ToString(item.ID), Convert.ToString(item["Title"]));
                        }
                        
                    }
                }
            }
            catch
            {
                //Error occured while getting all the owners of the alerts
            }
            return templatesByUser;
        }

        internal MailTemplate GetMailtemplateByID(string templateID)
        {
            MailTemplate mTemplate = null;
            try
            {
                SPListItem item = this.mailTemplateList.GetItemById(Convert.ToInt32(templateID));
                mTemplate = this.GetMailTemplateFromListItem(item);
            }
            catch { //Error occured while getting template by its id
            }
            return mTemplate;
        }

        internal void DeleteTemplateByID(string templateID)
        {
            try
            {
                SPListItem item = this.mailTemplateList.GetItemById(Convert.ToInt32(templateID));
                if (item != null)
                {
                    item.ParentList.ParentWeb.AllowUnsafeUpdates = true;
                    item.Delete();
                    item.ParentList.ParentWeb.AllowUnsafeUpdates = false;
                }
            }
            catch { //Errror occured while deleting template
            }
        }

        #endregion

        #region Template Usage Related

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

        internal  MailTemplateUsageObject GetMailTemplateUsageObjectFromListItem(SPListItem listItem)
        {

            MailTemplateUsageObject mObject = new MailTemplateUsageObject();
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
                string strQuery = string.Format("<Where><And><Eq><FieldRef Name=\"{0}\"/><Value Type=\"Choice\">{1}</Value></Eq><Eq><FieldRef Name=\"Alert\" LookupId=\"TRUE\"/><Value Type=\"Lookup\">{2}</Value></Eq></And></Where>", "EventType", eventType.ToString(), alertId);
                query.Query = strQuery;
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

        internal void DeleteTemplateUsageObjects(string alertid)
        {
            try
            {
                //We need to get all the instances which are related alert id
                SPQuery query = new SPQuery();
                query.Query  = string.Format("<Where><Eq><FieldRef Name=\"Alert\" LookupId=\"TRUE\"/><Value Type=\"Lookup\">{0}</Value></Eq></Where>", alertid);
                SPListItemCollection items = this.mailTemlateUsageList.GetItems(query);
                for (int i = 0; i < items.Count; i++)
                {
                    this.mailTemlateUsageList.ParentWeb.AllowUnsafeUpdates = true;
                    items[i].Delete();
                    this.mailTemlateUsageList.ParentWeb.AllowUnsafeUpdates = false;
                }
            }
            catch
            {
                //error occured while deleting the template usage object
            }
        }

        #endregion
    }
}
