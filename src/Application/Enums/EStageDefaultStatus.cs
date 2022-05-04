using System.ComponentModel;

namespace Application.Enums
{
    public enum EStageDefaultStatus
    {
        [Description("Approve")]
        Approve,

        [Description("Reject")]
        Reject,

        [Description("In Review")]
        InReview
    }
}
