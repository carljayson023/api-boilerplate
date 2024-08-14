using FastEndpoints;
using send.api.Infrastructure;
using send.api.Shared.Exceptions;
using send.api.Shared.Extension;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
try
{
    // Add services to the container

    var configurationbuilder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
    builder.Services.AddSerilogConfiguration(configurationbuilder);
    //builder.AddElkConfiguration(); // using serilog and ELK
    builder.Services.AddFastEndpointConfiguration();
    builder.Services.AddPersistenceInfrastructure();
    builder.Services.AddDependencyInjection();
    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddValidators(Assembly.GetExecutingAssembly()); // this will handle the validation behavior

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