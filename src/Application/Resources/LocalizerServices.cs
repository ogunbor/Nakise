using Microsoft.Extensions.Localization;

namespace Application.Resources
{
    public class ValidationLocalizerService : IValidationLocalizerService
	{
		// Represents a service that provides localized strings
		private readonly IStringLocalizer _localizer;

		public ValidationLocalizerService(IStringLocalizerFactory localizer)
		{
			// Add the locale resource file
			var type = typeof(ValidationErrorResources);
			_localizer = localizer.Create(type);
		}

		public LocalizedString GetLocalizedString(string key)
		{
			return _localizer[key];
		}
    }

	public class RestErrorLocalizerService : IRestErrorLocalizerService
	{
		// Represents a service that provides localized strings
		private readonly IStringLocalizer _localizer;

		public RestErrorLocalizerService(IStringLocalizerFactory localizer)
		{
			// Add the locale resource file
			var type = typeof(RestErrorResources);
			_localizer = localizer.Create(type);
		}

		public LocalizedString GetLocalizedString(string key)
		{
			return _localizer[key];
		}
	}
}
