using System.ComponentModel;

namespace Application.Enums
{
    public enum EApplicantStatus
    {
        Pending,
        [Description("In Review")]
        InReview,
        Approved,
        Rejected,
        Failed,
        Graduated
    }

    public enum EUpdateApplicantStatus
    {
        Approve,
        Reject
    }
}
