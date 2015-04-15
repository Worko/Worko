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
        private IList<bool> _constrains;
                
        public ShiftsConstrains()
        {
            _constrains = new List<bool>();
            for (int i = 0; i < 21; i++)
            {
                _constrains.Add(false);
            }
        }

        public IList<bool> Constrains
        {
            get { return _constrains; }
            set { _constrains = value; }
        }

        public string WorkerId { get; set; }

        public int WSID { get; set; }
    }
}
