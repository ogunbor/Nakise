using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProgrammeManagerConfigration: IEntityTypeConfiguration<ProgrammeManager>
    {
        public void Configure(EntityTypeBuilder<ProgrammeManager> builder)
        {
            builder.HasKey(p => new { p.ManagerId, p.ProgrammeId });

            builder.HasOne(p => p.Manager)
                .WithMany(p => p.ProgrammeManagers)
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Programme)
                .WithMany(p => p.ProgrammeManagers)
                .HasForeignKey(p => p.ProgrammeId);
        }
    }
}
