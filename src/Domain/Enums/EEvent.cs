using System.ComponentModel;


namespace Domain.Enums
{
    public enum EEvent
    {
        [Description("Ongoing")]
        Ongoing,
        [Description("Upcoming")]
        Upcoming,
        [Description("Past")]
        Past,
        [Description("Cancelled")]
        Cancelled,
    }
}
