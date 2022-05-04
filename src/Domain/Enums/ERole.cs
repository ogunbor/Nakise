using System.ComponentModel;

namespace Domain.Enums
{
    public enum ERole
    {
        [Description("Admin")]
        Admin = 1,
        [Description("ProgramManager")]
        ProgramManager = 2,
        [Description("Facilitator")]
        Facilitator = 3,
        [Description("Mentor")]
        Mentor = 4,
        [Description("Beneficiary")]
        Beneficiary = 5,
        [Description("Sponsor")]
        Sponsor = 6,
        [Description("Partner")]
        Partner = 7,
        [Description("Volunteer")]
        Volunteer = 8,
        [Description("Alumni")]
        Alumni = 9,
        [Description("Job Partner")]
        JobPartner = 10
    }
}
