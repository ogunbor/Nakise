using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.SuccessMessageTitle).IsRequired();
            builder.Property(x => x.SuccessMessageBody).IsRequired();

            builder.HasOne(x => x.Programme)
                .WithMany()
                .HasForeignKey(x => x.ProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Activity)
                .WithMany()
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
