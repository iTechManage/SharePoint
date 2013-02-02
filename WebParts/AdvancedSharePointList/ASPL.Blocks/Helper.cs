using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace ASPL.Blocks
{
    public class Helper
    {
        public static DataColumn CreateAutoRowIDColumn()
        {
            DataColumn col = new DataColumn(Constants.RowID, typeof(int));
            col.AutoIncrement = true;
            col.AutoIncrementSeed = 1;
            col.AutoIncrementStep = 1;
            return col;
        }

        public static bool IsValidXml(string xml)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.LoadXml(xml);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static XmlDocument LoadXml(string path)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            return xDoc;
        }

        public static bool ConvertToBool(string value)
        {
            bool result = false;
            Boolean.TryParse(value, out result);
            return result;
        }

        public static XmlDocument GetConfigFile(SPList list, string filename)
        {
            try
            {
                SPFile file = list.ParentWeb.GetFile(SPUtility.GetFullUrl(list.ParentWeb.Site, list.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename));
                if (file.Exists)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file.OpenBinaryStream());
                    return doc;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception exp)
            {
                Logging.Log(exp);
            }

            return null;
        }

        public static bool CreateConfigFile(SPList list, string filename, string xmlData)
        {
            try
            {
                string fileURL = SPUtility.GetFullUrl(list.ParentWeb.Site, list.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename);
                Byte[] contentArray = Encoding.ASCII.GetBytes(xmlData);
                SPFile file = list.ParentWeb.Files.Add(fileURL, contentArray, true);
                file.Update();
                return true;
            }

            catch (Exception exp)
            {
                Logging.Log(exp);
            }

            return false;
        }

        public static bool DeleteConfigFile(SPList list, string filename, string xmlData)
        {
            try
            {
                SPFile file = list.ParentWeb.GetFile(SPUtility.GetFullUrl(list.ParentWeb.Site, list.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename));
                if (file.Exists)
                {
                    file.Delete();
                    file.Update();
                    return true;
                }
                else
                {
                    return true;
                }
            }

            catch (Exception exp)
            {
                Logging.Log(exp);
            }
            return false;
        }

        public static string GetListSettingsURL(SPList list)
        {
            return SPUtility.GetFullUrl(list.ParentWeb.Site, list.ParentWebUrl.TrimEnd('/') + "/_layouts/listedit.aspx?List=" + SPHttpUtility.UrlKeyValueEncode(list.ID.ToString()));
        }

        public static IEnumerable<SPField> GetOrderedListField(SPList list)
        {
            var orderedFields = (from SPField field in list.Fields
                                 where !field.Hidden && !field.ReadOnlyField //&& field.CanBeDisplayedInEditForm
                                 orderby field.Title
                                 select field);

            return orderedFields;
        }

        public static DataRow GetRowFromDataTable(DataTable dataTable,int rowID)
        {
            DataRow results = (from myRow in dataTable.AsEnumerable()
                           where myRow.Field<int>(Constants.RowID) == rowID
                           select myRow).FirstOrDefault<DataRow>();

            return results;
        }

        public static DataView GetViewFromDataTable(DataTable dataTable, int rowID, string columnName)
        {
            var results = from myRow in dataTable.AsEnumerable()
                          where myRow.Field<int>(columnName) == rowID
                                    select myRow;


            return results.AsDataView();

        }

        public static string ConditionsToString(DataTable dataTable, int rowID, string columnName)
        {
            string conditions = "";
            DataTable dataTableCondition = GetViewFromDataTable(dataTable,rowID,columnName).ToTable();

            if (dataTableCondition != null && dataTableCondition.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTableCondition.Rows)
                {
                    conditions += dr[Constants.ConditionField.SPFieldDisplayName].ToString() + " " + dr[Constants.ConditionField.SPFieldOperatorName].ToString() + " " + dr[Constants.ConditionField.Value].ToString() + " AND ";
                }
            }

            return conditions.TrimEnd(" AND ".ToCharArray());

        }

        public static string HtmlEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return SPHttpUtility.HtmlEncode(value);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string HtmlDecode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return SPHttpUtility.HtmlDecode(value);
            }
            else
            {
                return string.Empty;
            }
        }

        //TODO: Replace the xml inner text separator
        //public static string[] XmlElementTextSplit(string value)
        //{
        //    return value.Split(Constants.XmlElementTextSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //}

        //TODO: Repalce the sp principle separator
        //public static string[] SPPrinciplesSplit(string value)
        //{
        //    return value.Split(Constants.SPPrinciplesSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //}

        //TODO: Always replace the textbox value with Helper.ReplaceInvalidChar() method
        public static string ReplaceInvalidChar(string value)
        {
            foreach (string c in Constants.InvalidChar)
            {
                value = value.Replace(c, string.Empty);
            }

            return value;
        }

        public static string ExtractHtml(string source)
        {
            try
            {
                XmlDocument x = new XmlDocument();
                source = SPHttpUtility.HtmlDecode(source);

                x.LoadXml(source);

                return x.InnerText;
            }
            catch (Exception)
            {
                return "";
            }
            
        }

        public static string GetFieldDisplayNames(SPList fromList,string fieldInternalNames)
        {
            string fieldDisplayName = "";

            foreach (string fieldInternalName in fieldInternalNames.Split(Constants.FieldToStringSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (fromList.Fields.ContainsField(fieldInternalName))
                    fieldDisplayName += fromList.Fields.GetFieldByInternalName(fieldInternalName) + Constants.FieldToStringSeparator;
            }

            return fieldDisplayName;

        }

    }
}
