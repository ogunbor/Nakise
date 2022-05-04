using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EProgrammeStatus
    {
        [Description("Ongoing")]
        Ongoing,
        [Description("Completed")]
        Completed,
        [Description("Not Started")]
        NotStarted,
        [Description("Up Coming")]
        Upcoming,
        Cancelled
    }
}
