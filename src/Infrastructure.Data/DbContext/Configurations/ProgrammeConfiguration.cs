using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProgrammeConfiguration : IEntityTypeConfiguration<Programme>
    {
        public void Configure(EntityTypeBuilder<Programme> builder)
        {
            builder.Property(x => x.Title).IsRequired()
                .HasMaxLength(125);
            builder.Property(x => x.Category).IsRequired()
                .HasMaxLength(125);
            builder.Property(x => x.DeliveryMethod).IsRequired()
                .HasMaxLength(64);
            builder.Property(x => x.Sponsor).IsRequired()
                .HasMaxLength(125);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.Country).IsRequired()
                .HasMaxLength(125);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
