using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace CrowCanyon.CascadedLookup
{
    class Utilities
    {
        #region Excluded Fields
        static readonly string[] EXCLUDED_FIELDS = new string[]{
              "_Author","_Category", "_CheckinComment", "_Comments", "_Contributor", "_Coverage", "_DCDateCreated",
              "_DCDateModified", "_EditMenuTableEnd", "_EditMenuTableStart", "_EndDate", "_Format",
              "_HasCopyDestinations", "_IsCurrentVersion", "_LastPrinted", "_Level", "_ModerationComments",
              "_ModerationStatus", "_Photo", "_Publisher", "_Relation", "_ResourceType", "_Revision",
              "_RightsManagement", "_SharedFileIndex", "_Source", "_SourceUrl", "_Status", "ActualWork",
              "AdminTaskAction", "AdminTaskDescription", "AdminTaskOrder", "AssignedTo", "Attachments",
              "AttendeeStatus",/* "Author",*/ "BaseAssociationGuid", "BaseName", "Birthday", "Body",
              "BodyAndMore", "BodyWasExpanded", "Categories", "CheckoutUser", "Comment", "Comments", "Completed",
              /*"Created",*/ "Created_x0020_By", "Created_x0020_Date", "DateCompleted", "DiscussionLastUpdated",
              "DiscussionTitle", "DocIcon", "DueDate",/* "Editor", */"EmailBody", "EmailCalendarDateStamp",
              "EmailCalendarSequence", "EmailCalendarUid", "EndDate", "EventType", "Expires",
              "ExtendedProperties", "fAllDayEvent", "File_x0020_Size", "File_x0020_Type", "FileDirRef",
              "FileLeafRef", "FileRef", "FileSizeDisplay", "FileType", "FormData", "FormURN", "fRecurrence",
              "FSObjType", "FullBody", "Group", "GUID", "HasCustomEmailBody", "Hobbies", "HTML_x0020_File_x0020_Type",
              "IMAddress", "ImageCreateDate", "ImageHeight", "ImageSize", "ImageWidth", "Indentation", "IndentLevel",
              "InstanceID", "IsActive", "IsSiteAdmin", "ItemChildCount", "Keywords", "Last_x0020_Modified","LessLink",
              "LimitedBody", "LinkDiscussionTitle", "LinkDiscussionTitleNoMenu", "LinkFilename", "LinkFilenameNoMenu",
              "LinkIssueIDNoMenu", "LinkTitle", "LinkTitleNoMenu","MasterSeriesItemID", "MessageBody", "MessageId",
              "MetaInfo",/* "Modified", */"Modified_x0020_By","MoreLink", "Notes", "Occurred", "ol_Department",
              "ol_EventAddress", "owshiddenversion", "ParentFolderId", "ParentLeafName", "ParentVersionString",
              "PendingModTime", "PercentComplete", "PermMask", "PersonViewMinimal", "Picture", "PostCategory",
              "Priority", "ProgId", "PublishedDate", "QuotedTextWasExpanded", "RecurrenceData", "RecurrenceID",
              "RelatedIssues", "RelevantMessages", "RepairDocument", "ReplyNoGif", "RulesUrl", "ScopeId", "SelectedFlag",
              "SelectFilename", "ShortestThreadIndex", "ShortestThreadIndexId", "ShortestThreadIndexIdLookup",
              "ShowCombineView", "ShowRepairView", "StartDate", "StatusBar", "SystemTask", "TaskCompanies",
              "TaskDueDate", "TaskGroup", "TaskStatus", "TaskType", "TemplateUrl", "ThreadIndex", "Threading",
              "ThreadingControls", "ThreadTopic", "Thumbnail", "TimeZone", "ToggleQuotedText", "TotalWork",
              "TrimmedBody", "UniqueId", "VirusStatus", "WebPage", "WorkAddress", "WorkflowAssociation",
              "WorkflowInstance", "WorkflowInstanceID", "WorkflowItemId", "WorkflowListId", "WorkflowVersion",
              "xd_ProgID", "xd_Signature", "XMLTZone", "XomlUrl","FolderChildCount"
        };
        #endregion

        public static bool IsLookupType(SPField field)
        {

            return (field != null && (field.Type == SPFieldType.Lookup || field.TypeAsString == "Lookup" || field.TypeAsString == "CCSCascadedLookup"));
            
        }

        public static bool IsDisplayField(SPField field)
        {
            bool display = false;

            if (field != null && !field.Hidden && (Array.IndexOf<string>(EXCLUDED_FIELDS, field.InternalName) < 0))
            {
                switch (field.Type)
                {
                    case SPFieldType.Computed:
                        if (((SPFieldComputed)field).EnableLookup) { display = true; }
                        break;
                    case SPFieldType.Calculated:
                        if (((SPFieldCalculated)field).OutputType == SPFieldType.Text) { display = true; }
                        break;
                    default:
                        display = true;
                        break;
                }
            }

            return display;
        }

        public static bool GeneralFields(SPField f)
        {
            if (f.InternalName.Equals("ID") || f.InternalName.Equals("Created") || f.InternalName.Equals("Author") || f.InternalName.Equals("Modified") ||
                f.InternalName.Equals("Editor") || f.InternalName.Equals("_UIVersionString") || f.InternalName.Equals("Title"))
            {
                return true;
            }
            else if (IsDisplayField(f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
