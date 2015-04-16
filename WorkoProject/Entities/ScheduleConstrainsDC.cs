using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ScheduleConstrainsDC
    {
        public int PKID                 { get; set; }
        public int WSID                 { get; set; }
        public int Day                  { get; set; }
        public StationStatus Status     { get; set; }
        public PartOfDay ShiftTime      { get; set; }
        public DateTime StartDay        { get; set; }
        public List<StationDC> Stations { get; set; }
    }
}
