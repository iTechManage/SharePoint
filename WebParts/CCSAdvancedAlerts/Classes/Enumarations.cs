using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{

    public enum ConditionComparisionType
    {
        Always,
        AfterChange
    }

    public enum AlertEventType
    {
        ItemAdded,
        ItemUpdated,
        ItemDeleted,
        DateColumn
    }
    public enum WeekDays
    {
        sun,
        mon,
        tue,
        wed,
        thu,
        fri,
        sat
        
    }

      public enum PeriodType
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }


    public enum PeriodPosition
    {
        Before,
        After
    }

    public enum RepeatType
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    public enum SendType
    {
        Immediate,
        Daily,
        Weekely
    }

    public enum UnionType
    {
        And,
        Or
    }

    public enum Operators
    {
        Eq,
        Neq,
        Contains,
        NotContains,
        Gt,
        Lt,
        Geq,
        Leq,
        Yes,
        No
    }

}
