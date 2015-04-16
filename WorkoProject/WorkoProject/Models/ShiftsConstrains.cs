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
                
        public ShiftsConstrains()
        {
            Constrains = new List<bool>();
            for (int i = 0; i < 21; i++)
            {
                Constrains.Add(false);
            }
        }

        public List<bool> Constrains { get; set; }

        public string WorkerId { get; set; }

        public int WSID { get; set; }
    }
}
