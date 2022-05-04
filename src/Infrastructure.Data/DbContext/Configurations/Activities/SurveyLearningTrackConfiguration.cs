using Domain.Entities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class SurveyLearningTrackConfiguration : IEntityTypeConfiguration<SurveyLearningTrack>
    {
        public void Configure(EntityTypeBuilder<SurveyLearningTrack> builder)
        {
            builder.HasKey(x => new { x.SurveyId, x.LearningTrackId });
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
