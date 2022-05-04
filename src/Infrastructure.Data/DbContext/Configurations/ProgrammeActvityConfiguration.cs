using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Activities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProgrammeActvityConfiguration : IEntityTypeConfiguration<ProgrammeActivity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProgrammeActivity> builder)
        {
            builder.ToView(null);
        }
    }
}
