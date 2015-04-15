using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// TODO: add necessary statuses
    public enum StationStatus
    {
        None = 0,
        Active = 1,
        Inactive = 2,
        Treatment = 3
    }

    public enum PartOfDay : int
    {
        Morning = 0,
        Evening = 1,
        Night = 2
    };
}
