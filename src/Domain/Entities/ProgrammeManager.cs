using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProgrammeManager
    {
        public Guid ManagerId { get; set; }
        public Guid ProgrammeId { get; set; }
        public User Manager { get; set; }
        public Programme Programme { get; set; }
    }
}
