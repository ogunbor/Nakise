using Domian.Entities.ActivityForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations.DefaultForms
{
    public class FormFieldValueConfiguration : IEntityTypeConfiguration<FormFieldValue>
    {
        public void Configure(EntityTypeBuilder<FormFieldValue> builder)
        {
        }
    }
}