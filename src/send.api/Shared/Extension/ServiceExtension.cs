
using send.api.Shared.Authentication.Config;
using FastEndpoints;
using FastEndpoints.Swagger;
using send.api.ServiceManager;
using Serilog.Formatting.Compact;
using Serilog;

namespace send.api.Shared.Extension
{
    public static class ServiceExtension
    {
        public static void AddFastEndpointConfiguration(this IServiceCollection services)
        {
            services.AddFastEndpoints()
                    .SwaggerDocument(
                        o =>
                        {
                            o.DocumentSettings =
                                s =>
                                {
                                    s.DocumentName = "Release 1.0";
                                    s.Title = "Send API";
                                    s.Version = "v1.0";
                                };
                            o.MaxEndpointVersion = 1;
                            o.RemoveEmptyRequestSchema = false;
                            o.TagStripSymbols = true;
                        });

            #region use this if new version has added
            //.SwaggerDocument(
            //    o =>
            //    {
            //        o.DocumentSettings =
            //            s =>
            //            {
            //                s.DocumentName = "Release 2.0";
            //                s.Title = "FastEndpoints Sandbox";
            //                s.Version = "v2.0";
            //            };
            //        o.MaxEndpointVersion = 2;
            //        o.ShowDeprecatedOps = true;
            //        o.RemoveEmptyRequestSchema = false;
            //        o.TagStripSymbols = true;
            //    })
            //.SwaggerDocument(
            //    o =>
            //    {
            //        o.DocumentSettings =
            //            s =>
            //            {
            //                s.DocumentName = "Release 3.0";
            //                s.Title = "FastEndpoints Sandbox ver3 only";
            //                s.Version = "v3.0";
            //            };
            //        o.MinEndpointVersion = 3;
            //        o.MaxEndpointVersion = 3;
            //        o.ExcludeNonFastEndpoints = true;
            //    });
            #endregion
        }

        public static void AddOAuth2(this IServiceCollection services, IConfiguration config)
        {
            string keycloakPublicKey = File.ReadAllText(AppContext.BaseDirectory + (config.GetValue<string>("PUBLIC-KEY-PATH") ?? @"/Shared/Authentication/Docs/Keycloak/DEV/public-keycloak.xml"));

            var issuerLink = Environment.GetEnvironmentVariable("JWT_ISSUER_KEYCLOAK");

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
            });

            services.AddChassis(config, confg =>
            {
                confg.AddAuth(new JWTAuthentication(), keycloakPublicKey, issuerLink);
            });
        }
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IWeatherForeCastsService, WeatherForeCastsService>();
        }

        public static void AddSerilogConfiguration(this IServiceCollection services)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    new RenderedCompactJsonFormatter(),
                    Environment.GetEnvironmentVariable("LOG_FILE_BASE_PATH") + "Send-API_.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: null,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    rollOnFileSizeLimit: true
                )
                .CreateLogger();
        }
    }
}
