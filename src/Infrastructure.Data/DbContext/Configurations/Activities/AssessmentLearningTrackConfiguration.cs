using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class AssessmentLearningTrackConfiguration : IEntityTypeConfiguration<AssessmentLearningTrack>
    {
        public void Configure(EntityTypeBuilder<AssessmentLearningTrack> builder)
        {
            builder.HasKey(x => new { x.AssessmentId, x.LearningTrackId });
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
