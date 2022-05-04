using API.Extensions;
using API.Middlewares;
using Application.Helpers;
using Application.Services.V1;
using Hangfire;
using Infrastructure.Data.DbContext;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureGlobalization();
builder.Services.ConfigureIisIntegration();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureIOObjects(builder.Configuration);
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager(builder.Configuration);
builder.Services.ConfigureApplicationService();
builder.Services.ConfigureAWSServices(builder.Configuration);
builder.Services.AddControllers()
    .AddXmlDataContractSerializerFormatters();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureApiVersioning(builder.Configuration);
builder.Services.ConfigureMvc();
builder.Services.AddHostedService<AppBackgroundService>();

builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://c4a6236685c4436b9e33716b763941d2@o373456.ingest.sentry.io/6176454";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
    // We recommend adjusting this value in production.
    o.TracesSampleRate = 1.0;
    o.Environment = "development";
});

builder.Services.ConfigureHangfire(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.SeedRoleData().Wait();
app.SeedOrganizationUserData().Wait();
app.SeedActivityData().Wait();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

var localizeOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizeOptions.Value);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard();
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangFireAuthorizationFilter(builder.Configuration) }
    });
}
app.UseErrorHandler();

app.MapControllers();
WebHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.Run();
