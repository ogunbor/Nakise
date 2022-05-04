using System.ComponentModel;

namespace Domain.Enums
{
    public enum ETokenType
    {
        [Description("Create New User")]
        InviteUser,
        [Description("Reset Password")]
        ResetPassword
    }
}
