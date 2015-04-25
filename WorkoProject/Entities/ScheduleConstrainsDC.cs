using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ScheduleConstrainsDC
    {
        public ScheduleConstrainsDC()
        {
        }

        public int PKID                             { get; set; }
        public int WSID                             { get; set; }
        public int StationId                        { get; set; }
        public StationStatus Status                 { get; set; }
        public List<StationConstrains> Constrains   { get; set; }
    }

    public partial class StationConstrains
    {
        public int Priority         { get; set; }
        public int NumberOfWorkers  { get; set; }
        public StationStatus Status { get; set; }
        public int Day        { get; set; }
        public int ShiftTime  { get; set; }
    }
}
