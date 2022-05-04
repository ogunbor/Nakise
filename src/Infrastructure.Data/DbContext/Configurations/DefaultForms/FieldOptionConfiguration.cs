using Domian.Entities.ActivityForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.DefaultForms
{
    public class FieldOptionConfiguration : IEntityTypeConfiguration<FieldOption>
    {
        public void Configure(EntityTypeBuilder<FieldOption> builder)
        {
            builder.Property(x => x.Key).IsRequired();
            builder.Property(x => x.Value).IsRequired();
            builder.HasOne(x => x.FormField)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.FormFieldId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}