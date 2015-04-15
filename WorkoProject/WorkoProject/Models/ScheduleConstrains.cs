using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkoProject.Models
{
    public class ScheduleConstrains
    {
        public bool[][] _constrains;
        
        public ScheduleConstrains()
        {

            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 3; j++)
                        _constrains[i][j] = false;

            StartDay = DateTime.Now.AddDays(-1 * ((int)DateTime.Now.DayOfWeek));
        }

        public int PKID              { get; set; }
        public int WSID              { get; set; }
        public int StationId         { get; set; }
        public int Day               { get; set; }
        public StationStatus Status  { get; set; }
        public PartOfDay ShiftTime   { get; set; }
        public DateTime StartDay     { get; set; }
    }
}