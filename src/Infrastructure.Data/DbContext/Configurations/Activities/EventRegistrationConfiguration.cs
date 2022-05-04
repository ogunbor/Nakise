using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
    {
        public void Configure(EntityTypeBuilder<EventRegistration> builder)
        {
            builder.HasKey(p => new { p.ApprovedApplicantId, p.EventId });

            builder.HasOne(x => x.Event)
               .WithMany()
               .HasForeignKey(x => x.EventId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ApprovedApplicant)
               .WithMany()
               .HasForeignKey(x => x.ApprovedApplicantId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
