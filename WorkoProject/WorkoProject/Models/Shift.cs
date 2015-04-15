using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Models
{
    public class Shift
    {
        public DateTime Date { get; set; }

        public DayOfWeek Day { get; set; }
        
        public PartOfDay Part { get; set; }

        public bool IsActive { get; set; }

        public static int GetShiftIndex(DayOfWeek day, PartOfDay part)
        {
            return ((int)part) * 7 + (int)day;
        }
    }
}
