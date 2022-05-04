using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LearningTrackFacilitator
    {
        public Guid LearningTrackId { get; set; }
        public Guid FacilitatorId { get; set; }
        public LearningTrack LearningTrack { get; set; }
        public User Facilitator { get; set; }
    }
}
