using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class WorkSchedule
    {
        public WorkSchedule(int wsid)
        {
            WSID = wsid;
            Template = new WorkScheduleTemplate();
        }

        public int WSID { get; set; }

        public WorkScheduleTemplate Template { get; set; }

    }

}
