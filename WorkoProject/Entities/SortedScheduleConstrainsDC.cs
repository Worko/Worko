using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SortedScheduleConstrainsDC
    {
        public int StationId        { get; set; }
        public StationStatus Status { get; set; }
        public int Priority         { get; set; }
        public int NumberOfWorkers  { get; set; }
        public int Day              { get; set; }
        public int ShiftTime        { get; set; }
    }

}
