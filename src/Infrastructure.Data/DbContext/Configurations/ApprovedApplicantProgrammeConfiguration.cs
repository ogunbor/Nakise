using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ApprovedApplicantProgrammeConfiguration : IEntityTypeConfiguration<ApprovedApplicantProgramme>
    {
        public void Configure(EntityTypeBuilder<ApprovedApplicantProgramme> builder)
        {
            builder.HasKey(p => new { p.ApprovedApplicantId, p.ProgrammeId });

            builder.HasOne(l => l.learningTrack)
                .WithMany()
                .HasForeignKey(l => l.LearningTrackId);

            builder.HasOne(p => p.Programme)
                        .WithMany()
                        .HasForeignKey(p => p.ProgrammeId);
        }     
    }
}
