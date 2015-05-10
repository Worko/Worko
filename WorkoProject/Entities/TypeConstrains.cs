using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class TypeConstrains
    {
        public TypeConstrains()
        {
            Constrains = new List<bool>();
            for (int i = 0; i < 21; i++)
            {
                Constrains.Add(false);
            }
        }

        public IList<bool> Constrains { get; set; }

        public int TypeId { get; set; }
    }
}
