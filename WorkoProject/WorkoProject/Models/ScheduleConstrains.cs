using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkoProject.Models
{
    public class ScheduleConstrains
    {
        public ScheduleConstrains()
        {
            Stations = new List<Station>();
            StartDay = DateTime.Now.AddDays(-1 * ((int)DateTime.Now.DayOfWeek));
        }

        public int PKID               { get; set; }
        public int WSID               { get; set; }
        public int Day                { get; set; }
        public StationStatus Status   { get; set; }
        public PartOfDay ShiftTime    { get; set; }
        public DateTime StartDay      { get; set; }
        public List<Station> Stations { get; set; }
    }
}