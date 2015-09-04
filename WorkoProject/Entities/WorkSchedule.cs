using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class WorkSchedule
    {
        public WorkSchedule(int wsid, DateTime startDate)
        {
            WSID = wsid;
            Schedule = new WorkScheduleTemplate();
            StartDate = startDate;
        }

        public int WSID { get; set; }
        public DateTime StartDate { get; set; }
        public WorkScheduleTemplate Schedule { get; set; }
        public double Capacity { get; set; }

    }

}
