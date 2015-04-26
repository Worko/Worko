using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class Settings
    {
        public int LimitWorkerConstrains { get; set; }

        public int MinShiftsToAssign { get; set; }

        public int MaxShiftsToAssign { get; set; }

        public int MaxNightShiftsForThreeWeeks { get; set; }

        public int MaxHoursInWeek { get; set; }

        public int DeadLineDayToSendConstrains { get; set; }


    }
}