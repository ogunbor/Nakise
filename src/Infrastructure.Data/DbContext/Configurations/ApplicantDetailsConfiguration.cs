using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ApplicantDetailsConfiguration : IEntityTypeConfiguration<ApplicantDetail>
    {
        public void Configure(EntityTypeBuilder<ApplicantDetail> builder)
        {
            builder.HasIndex(x => x.ActivityId);
            builder.HasOne(x => x.Form)
                .WithMany()
                .HasForeignKey(x => x.FormId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.LearningTrack)
                .WithMany()
                .HasForeignKey(x => x.LearningTrackId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Stage)
                .WithMany()
                .HasForeignKey(x => x.StageId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
