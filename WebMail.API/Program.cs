using Microsoft.OpenApi.Models;
using Serilog;
using WebMail.API.Services;
using WebMail.API.Interfaces;
using WebMail.Infrastructure.Interfaces;
using WebMail.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetValue<string>("DatabaseSettings:ConnectionString");
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


static void ConfigureServices(IServiceCollection services)
{
    // add Serilog as the log provider.
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog();
    });

    services.AddSingleton<IEmailRepository, EmailRepository>();
    services.AddScoped<ICreateEmailService, CreateEmailService>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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

static void ConfigureApplication(WebApplication app)
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

static void ConfigureSerilog(WebApplicationBuilder builder, string connectionString)
{
    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();
}