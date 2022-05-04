using Domian.Entities.ActivityForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.DefaultForms
{
    public class ActivityFormConfiguration : IEntityTypeConfiguration<ActivityForm>
    {
        public void Configure(EntityTypeBuilder<ActivityForm> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.HasMany(x => x.FormFields)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}