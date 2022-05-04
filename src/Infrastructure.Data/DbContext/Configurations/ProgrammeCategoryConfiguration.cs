using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProgrammeCategoryConfiguration: IEntityTypeConfiguration<ProgrammeCategory>
    {
        public void Configure(EntityTypeBuilder<ProgrammeCategory> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(125);
        }
    }
}
