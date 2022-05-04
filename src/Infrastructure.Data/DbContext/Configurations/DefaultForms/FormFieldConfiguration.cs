using Domian.Entities.ActivityForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.DefaultForms
{
    public class FormFieldConfiguration : IEntityTypeConfiguration<FormField>
    {
        public void Configure(EntityTypeBuilder<FormField> builder)
        {
            builder.Property(x => x.Key).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Label).IsRequired();
            builder.Property(x => x.Required).IsRequired();
        }
    }
}