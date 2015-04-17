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
            Constrains = new bool[7][];
            for (int i = 0; i < 7; i++)
                Constrains[i] = new bool[3];
        }

        public int PKID                 { get; set; }
        public int WSID                 { get; set; }
        public int StationId            { get; set; }
        public StationStatus Status     { get; set; }
        public bool[][] Constrains      { get; set; }
    }
}
