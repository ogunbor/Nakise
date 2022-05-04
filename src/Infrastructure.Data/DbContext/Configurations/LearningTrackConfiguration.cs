using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class LearningTrackConfiguration : IEntityTypeConfiguration<LearningTrack>
    {
        public void Configure(EntityTypeBuilder<LearningTrack> builder)
        {
            builder.Property(x => x.Title).IsRequired()
                .HasMaxLength(125);
        }
    }
}
