using System.Reflection;
using System.Text;
using System.Text.Json;
using Amazon.Util.Internal.PlatformServices;
using Domain.Environments;
using Domain.Transport;
using Infrastructure.Evironments;
using Infrastructure.Transport;
using Main.Service.ExtensionService;
using Main.Service.MiddleWare;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
// Add services to the container.
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(builder.Configuration);
    cfg.WriteTo.Console(theme: AnsiConsoleTheme.Grayscale);
});

builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false); // needed for mediatr DI;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddSingleton<IEnvironmentsConfig, EnvironmentsConfig>();
builder.Services.AddSingleton<IRestAPI, RestAPI>();

builder.Services.AddOptions();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
});
;

builder.Services.AddMemoryCache();
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddLazyCache();
builder.Services.AddMongoDb(builder.Configuration);


builder.Services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));



await using var app = builder.Build();

app.UseMyMiddleware();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

//app.UseHttpsRedirection();

app.UseRouting();

//app.UseAuthorization();

app.UseResponseCompression();
//app.UseSerilogRequestLogging();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = WriteResponse
    });
});



await app.RunAsync("http://*:8080");

static Task WriteResponse(HttpContext context, HealthReport result)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions
    {
        Indented = true
    };

    using var stream = new MemoryStream();
    using (var writer = new Utf8JsonWriter(stream, options))
    {
        writer.WriteStartObject();
        writer.WriteString("status", "success");
        writer.WriteString("message", "OK");
        writer.WriteNull("data");
        writer.WriteEndObject();
    }

    var json = Encoding.UTF8.GetString(stream.ToArray());

    return context.Response.WriteAsync(json);
}