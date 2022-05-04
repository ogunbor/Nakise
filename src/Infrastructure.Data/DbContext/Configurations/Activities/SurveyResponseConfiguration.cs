using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyResponse> builder)
        {
            builder.HasKey(x => new { x.SurveyId, x.ProgramId, x.ApprovedApplicantId });
            builder.HasOne(x => x.Programme)
              .WithMany()
              .HasForeignKey(x => x.ProgramId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Survey)
              .WithMany()
              .HasForeignKey(x => x.SurveyId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ApprovedApplicant)
              .WithMany()
              .HasForeignKey(x => x.ApprovedApplicantId)
              .OnDelete(DeleteBehavior.NoAction);

        }
    }
}