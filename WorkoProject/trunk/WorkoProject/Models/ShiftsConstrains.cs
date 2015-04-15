using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoProject.Models
{
    public class ShiftsConstrains
    {
        private List<bool> _constrains;
                
        public ShiftsConstrains()
        {
            _constrains = new List<bool>();
            for (int i = 0; i < 21; i++)
            {
                _constrains.Add(false);
            }

            StartDay = DateTime.Now.AddDays(-1 * ((int)DateTime.Now.DayOfWeek));
        }

        public List<bool> Constrains
        {
            get { return _constrains; }
        }

        public string WorkerId { get; set; }

        public DateTime StartDay { get; set; }

    }
}
