using Domain.Entities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class FormLearningTrackConfiguration : IEntityTypeConfiguration<FormLearningTrack>
    {
        public void Configure(EntityTypeBuilder<FormLearningTrack> builder)
        {
            builder.HasKey(x => new { x.FormId, x.LearningTrackId });
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
