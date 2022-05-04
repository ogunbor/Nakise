using Domain.Entities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class TrainingLearningTrackConfiguration : IEntityTypeConfiguration<TrainingLearningTrack>
    {
        public void Configure(EntityTypeBuilder<TrainingLearningTrack> builder)
        {
            builder.HasKey(x => new { x.TrainingId, x.LearningTrackId });
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
