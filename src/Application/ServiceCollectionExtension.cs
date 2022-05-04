using Application.Helpers;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceCollectionExtension
	{
		public static void AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IWebHelper, WebHelper>();
			services.AddScoped<IEmailManager, EmailManager>();
			services.AddTransient<IAwsS3Client, AwsS3Client>();
		}
	}
}
