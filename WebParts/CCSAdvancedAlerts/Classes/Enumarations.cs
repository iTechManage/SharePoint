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
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
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
        ImmediateAlways,
        ImmediateBusinessDays,
        Daily,
        Weekly
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

    public enum ConditionValueType
    {
        StringLiteral,
        ItemPropertyValue,
        Function,
        FunctionOnPropertyValue,
        Invalid
    }

    public enum GroupEvalType
    {
        And,
        Or
    }
}
