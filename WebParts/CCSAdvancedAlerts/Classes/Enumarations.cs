using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{
    public enum AlertEventType
    {
        ItemAdded,
        ItemUpdated,
        ItemDeleted,
        DateColumn
    }


    public enum PeriodType
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3,
        Months = 4,
        Years = 5
    }


    public enum PeriodPosition
    {
        Before = 0,
        After = 1
    }

    public enum RepeatType
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3,
        Months = 4,
        Years = 5
    }

    public enum SendType
    {
        Immediate,
        Daily,
        Weekely
    }

    public enum UnionType
    {
        And =0,
        Or =1
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
