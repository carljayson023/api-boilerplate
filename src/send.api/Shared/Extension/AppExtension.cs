using FastEndpoints;
using FastEndpoints.Swagger;
using send.api.Shared.Exceptions;
using send.api.Shared.NamingPolicy;
using Serilog;

namespace send.api.Shared.Extension
{
    public static class AppExtension
    {
        public static void UseFastEndpointExtension(this IApplicationBuilder app)
        {
            var subRoute = Environment.GetEnvironmentVariable("SUB_ROUTE") ?? "send-api";
            
            app.UseFastEndpoints(options =>
            {
                options.Validation.EnableDataAnnotationsSupport = true;

                options.Serializer.Options.PropertyNamingPolicy = new SnakeCaseNamingPolicy();

                options.Binding.ValueParserFor<Guid>(x => new(Guid.TryParse(x?.ToString(), out var res), res));

                options.Endpoints.RoutePrefix = subRoute;
                options.Endpoints.ShortNames = false;
                options.Endpoints.PrefixNameWithFirstTag = true;
                options.Endpoints.Filter = ep => ep.EndpointTags?.Contains("exclude") is not true;
                options.Endpoints.Configurator =
                    ep =>
                    {
                        if (ep.EndpointTags?.Contains("Orders") is true)
                            ep.Description(b => b.Produces<ErrorResponse>(400, "application/problem+json"));
                    };

                options.Versioning.Prefix = "v";
                options.Versioning.PrependToRoute = true;
                options.Throttle.HeaderName = "X-Custom-Throttle-Header";
                options.Throttle.Message = "Custom Error Response";
            }).UseSwaggerGen();

            #region to be resolve
            // Configure Job Queues
            //try
            //{
            //    // Configure Job Queues
            //    app.UseJobQueues(o =>
            //    {
            //        o.MaxConcurrency = 4;
            //        o.LimitsFor<JobTestCommand>(1, TimeSpan.FromSeconds(1));
            //        o.LimitsFor<JobCancelTestCommand>(100, TimeSpan.FromSeconds(60));
            //        o.StorageProbeDelay = TimeSpan.FromMilliseconds(100);
            //    });

            //    Log.Information("Job queues configured successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex, "An error occurred while configuring job queues.");
            //    throw; // Re-throw the exception to ensure the application does not continue in an invalid state
            //}
            #endregion
        }
    }
}
