using FastEndpoints;
using send.api.Infrastructure;
using send.api.Shared.Exceptions;
using send.api.Shared.Extension;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
try
{
    // Add services to the container
    builder.Services.AddSerilogConfiguration();
    builder.Services.AddFastEndpointConfiguration();
    builder.Services.AddPersistenceInfrastructure();
    builder.Services.AddDependencyInjection();
    builder.Services.AddAutoMapper(typeof(Program).Assembly);

    ConfigurationManager configuration = builder.Configuration;
    builder.Services.AddOAuth2(configuration);

    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandling>();

    app.UseFastEndpointExtension();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "!!!Program terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}