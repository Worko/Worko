using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Shift
    {
        public DateTime Date { get; set; }

        public DayOfWeek Day { get; set; }

        public PartOfDay Part { get; set; }

        public bool IsActive { get; set; }

        public List<Station> Stations { get; set; }

        public static int GetShiftIndex(DayOfWeek day, PartOfDay part)
        {
            return ((int)part) * 7 + (int)day;
        }

        public static int GetNextShiftIndex(DayOfWeek day, PartOfDay part)
        {
            int p = (int)part;
            int d = (int)day;

            if (p == 2)
            {
                p = 0;
                d++;
            }
            else
            {
                p++;
            }

            return p * 7 + d;
        }

        public static int GetPreviousShiftIndex(DayOfWeek day, PartOfDay part)
        {
            int p = (int)part;
            int d = (int)day;

            if (p == 0)
            {
                p = 2;
                d--;
            }
            else
            {
                p--;
            }

            return p * 7 + d;
        }
    }
}
