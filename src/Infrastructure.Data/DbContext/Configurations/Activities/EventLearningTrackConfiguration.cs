using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class EventLearningTrackConfiguration : IEntityTypeConfiguration<EventLearningTrack>
    {
        public void Configure(EntityTypeBuilder<EventLearningTrack> builder)
        {
            builder.HasKey(x => new { x.EventId, x.LearningTrackId });
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
