using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ShiftsConstrainsDC
    {
        private IList<bool> _constrains;

        public IList<bool> Constrains
        {
            get { return _constrains; }
            set { _constrains = value; }
        }

        public string WorkerId { get; set; }

        public int WSID { get; set; }
    }
}
