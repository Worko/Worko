using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class RequestDC
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool IsNewRequest { get; set; }

        public string WorkerId { get; set; }
    }
}
