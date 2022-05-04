using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProgrammeSponsorConfiguration: IEntityTypeConfiguration<ProgrammeSponsor>
    {
        public void Configure(EntityTypeBuilder<ProgrammeSponsor> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(125);
        }
    }
}
