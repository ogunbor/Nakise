using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class LearningTrackFacilitatorConfiguration: IEntityTypeConfiguration<LearningTrackFacilitator>
    {
        public void Configure(EntityTypeBuilder<LearningTrackFacilitator> builder)
        {
            builder.HasKey(x => new { x.FacilitatorId, x.LearningTrackId });

            builder.HasOne(x => x.Facilitator)
                .WithMany(x => x.LearningTrackFacilitators)
                .HasForeignKey(x => x.FacilitatorId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            builder.HasOne(x => x.LearningTrack)
                .WithMany(x => x.LearningTrackFacilitators)
                .HasForeignKey(x => x.LearningTrackId);
        }
    }
}
