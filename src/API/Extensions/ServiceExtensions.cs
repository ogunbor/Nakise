using Amazon.S3;
using API.Configurations;
using API.Middlewares;
using Application;
using Application.Contracts.V1;
using Application.Contracts.V1.Activities;
using Application.Resources;
using Application.Services.V1;
using Application.Services.V1.Activities;
using Application.Settings;
using Application.Validations;
using Domain.Entities;
using FluentValidation.AspNetCore;
using Hangfire;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Text;


namespace API.Extensions
{
    public static class ServiceExtensions
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }
        
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddHttpClient<AssessmentService>();
        }
            

        public static void ConfigureIisIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts
                    .UseLoggerFactory(ContextLoggerFactory)
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure()));
        }

        /// <summary>
        /// Configure binding of IConfigurations to typed object for better maintainability.    
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIOObjects(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AwsSettings>(configuration.GetSection(nameof(AwsSettings)));
            services.Configure<FileSettings>(configuration.GetSection(nameof(FileSettings)));
            services.Configure<AzureSettings>(configuration.GetSection(nameof(AzureSettings)));
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
                };
            });
        }

        public static void ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc()
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
                }).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<UserValidator>());
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(x => 
                x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
        }

        public static void ConfigureAWSServices(this IServiceCollection services, IConfiguration configuration)
        {
            // var options = configuration.GetAWSOptions();
            services.AddAWSService<IAmazonS3>(configuration.GetAWSOptions());
            services.AddTransient<IAwsS3Client, AwsS3Client>();
        }

        public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddMvcCore().AddApiExplorer();
        }

        /// <summary>
        /// Configure service that arent typeof (IAutoDependencyService) at the application layer
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureApplicationService(this IServiceCollection services)
        {
            services.AddApplicationServices();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RemoveVersionFromParameter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                // TODO: Fix the Docker error on this
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureGlobalization(this IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddSingleton<IValidationLocalizerService, ValidationLocalizerService>();
            services.AddSingleton<IRestErrorLocalizerService, RestErrorLocalizerService>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            }).AddRoles<Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        }
    }
}
