using Microsoft.Extensions.Localization;

namespace Application.Resources
{
    public interface IValidationLocalizerService
    {
        LocalizedString GetLocalizedString(string key);
    }
}
