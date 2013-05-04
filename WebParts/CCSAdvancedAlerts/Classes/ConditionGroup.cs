using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts
{
    [Serializable]
    class ConditionGroup
    {
        private GroupEvalType evalType;
        internal GroupEvalType GroupEvaluationType
        {
            get { return evalType; }
            set { evalType = value; }
        }

        private List<Condition> conditions = new List<Condition>();
        internal List<Condition> Conditions
        {
            // TODO: const?
            get { return conditions; }
        }

        internal void add_condition(Condition cond)
        {
            // TODO: Any checks?
            conditions.Add(cond);
        }

        internal List<ConditionGroup> sub_groups = new List<ConditionGroup>();
        internal List<ConditionGroup> SubGroups
        {
            get { return sub_groups; }
        }

        internal void add_sub_group(ConditionGroup grp)
        {
            this.sub_groups.Add(grp);
        }

        internal ConditionGroup(string xNode)
        {
            if (!string.IsNullOrEmpty(xNode))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xNode);
                BuildGroupFromXML(xDoc.DocumentElement);
            }
        }

        internal ConditionGroup(XmlNode xNode)
        {
            if (xNode != null)
                BuildGroupFromXML(xNode as XmlElement);
        }
        
        private void BuildGroupFromXML(XmlElement xmlElement)
        {
            try
            {
                this.GroupEvaluationType =
                    (GroupEvalType)Enum.Parse(typeof(GroupEvalType),
                    xmlElement.GetAttribute("Evaluation"));

                // Group must have sub-groups and/or conditions
                Debug.Assert(xmlElement.HasChildNodes);

                // TODO: check names and get them from resource
                foreach (XmlNode condition_node in xmlElement.GetElementsByTagName("Condition"))
                {
                    this.conditions.Add(new Condition(condition_node));
                }
                
                foreach (XmlNode group_node in xmlElement.GetElementsByTagName("Group"))
                {
                    this.sub_groups.Add(new ConditionGroup(group_node));
                }
            }
            catch
            {
                // TODO
            }
        }

        internal bool isValid(SPListItem item, AlertEventType eventType, SPItemEventProperties properties)
        {
            bool bReturn;

            if (evalType == GroupEvalType.And)
            {
                  bReturn = true;
            }
            else
            {
                  bReturn = false;
            }

            foreach (Condition cond in conditions)
            {
                bool cond_val = cond.isValid(item, eventType, properties);

                if (evalType == GroupEvalType.And && cond_val == false) return false;
                if (evalType == GroupEvalType.Or && cond_val == true) return true;
            }

            foreach (ConditionGroup grp in sub_groups)
            {
                bool grp_val = grp.isValid(item, eventType, properties);

                if (evalType == GroupEvalType.And && grp_val == false) return false;
                if (evalType == GroupEvalType.Or && grp_val == true) return true;
            }

            return bReturn;
        }
    }
}
