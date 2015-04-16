using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ShiftsConstrainsDC
    {
        private List<bool> _constrains;

        public List<bool> Constrains
        {
            get { return _constrains; }
            set { _constrains = value; }
        }

        public string WorkerId { get; set; }

        public int WSID { get; set; }
    }
}
