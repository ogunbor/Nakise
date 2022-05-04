using Microsoft.Extensions.Localization;

namespace Application.Resources
{
    public interface IRestErrorLocalizerService
    {
        LocalizedString GetLocalizedString(string key);
    }
}
