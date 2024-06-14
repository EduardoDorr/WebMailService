using System.Reflection;

using Microsoft.OpenApi.Models;

using Serilog;

using WebMail.Domain.Repositories;
using WebMail.Application.Options;
using WebMail.Application.Services;
using WebMail.Application.Interfaces;
using WebMail.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();

var configuration = builder.Configuration;
var connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
var port = configuration.GetValue<int>("ApiSettings:Port");

ConfigureSerilog(builder, connectionString);

try
{
    Log.Information("Iniciando o aplicativo.");
    // Add services to the container.
    ConfigureServices(builder.Services);
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    ConfigureApplication(app);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "O aplicativo encontrou um erro durante a execução.");
}
finally
{
    Log.CloseAndFlush();
}


void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog();
    });

    services.Configure<ServiceOptions>(options => configuration.GetSection(nameof(ServiceOptions)).Bind(options));

    services.AddAutoMapper(Assembly.GetExecutingAssembly());

    services.AddSingleton<IEmailRepository, EmailRepository>();
    services.AddScoped<ICreateEmailService, CreateEmailService>();
    services.AddHostedService<SendEmailService>();

    services.AddControllers();

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(s =>
    {
        s.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WebMail.API",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Name = "Eduardo Dörr",
                Email = "edudorr@hotmail.com",
                Url = new Uri("https://eduardodorr-portfolio.vercel.app/")
            }
        });
    });
}

void ConfigureApplication(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

void ConfigureSerilog(WebApplicationBuilder builder, string connectionString)
{
    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();
}