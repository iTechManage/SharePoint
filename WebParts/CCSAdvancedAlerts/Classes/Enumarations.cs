using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{
    public enum ReceivedEventType
    {
        ItemAdded = 0,
        ItemDeleted = 2,
        ItemUpdated = 1,
        DateTime=3,
        Custom = 4
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
        ImmediatelyAlways =0,
        ImmediatelyBusinessDays = 0,
        Daily = 0,
        Weekely = 0,
    }

    public enum UnionType
    {
        And =0,
        Or =1
    }
    


}
