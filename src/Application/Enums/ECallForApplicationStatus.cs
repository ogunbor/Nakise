using System.ComponentModel;

namespace Application.Enums
{
    public enum ECallForApplicationStatus
    {
        Activate,
        Deactivate
    }

    public enum ECallForApplicationOtherStatus
    {
        [Description("Not started")]
        NotStarted,
        [Description("Ongoing")]
        Ongoing,
        [Description("Completed")]
        Completed
    }
}
