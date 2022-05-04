using Domain.Entities.Actvities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.DbContext.Configurations.Activities
{
    public class AssessmentSessionConfiguration : IEntityTypeConfiguration<AssessmentSession>
    {
        public void Configure(EntityTypeBuilder<AssessmentSession> builder)
        {
            builder.HasKey(x => new { x.AssessmentId, x.ApprovedApplicantId });
        }
    }
}
