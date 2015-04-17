using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class StationDC
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StationStatus Status { get; set; }
        public bool[][] Constrains { get; set; }
    }
}
