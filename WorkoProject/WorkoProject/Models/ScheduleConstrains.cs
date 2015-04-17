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
            Constrains = new bool[7][];
            for (int i = 0; i < 7; i++)
                Constrains[i] = new bool[3];
        }

        public int PKID               { get; set; }
        public int WSID               { get; set; }
        public int StationId          { get; set; }
        public int Day                { get; set; }
        public StationStatus Status   { get; set; }
        public PartOfDay ShiftTime    { get; set; }
        public bool[][] Constrains    { get; set; }
    }
}