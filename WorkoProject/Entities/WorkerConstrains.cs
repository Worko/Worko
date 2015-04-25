using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class WorkerConstrains
    {

        public WorkerConstrains()
        {
            Constrains = new bool[7][];
            for (int i = 0; i < 7; i++)
                Constrains[i] = new bool[3];
        }

        public string WorkerID          { get; set; }
        public bool[][] Constrains      { get; set; }

    }
}
