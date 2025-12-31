using Serilog;

using WebMail.API.Configuration;

try
{
    Log.Information("Iniciando o aplicativo.");

    var app = WebApplication.CreateBuilder(args)
        .ConfigureSerilog()
        .ConfigureServices().Build()
        .ConfigureApplication();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "O aplicativo encontrou um erro durante a execu��o.");
}
finally
{
    await Log.CloseAndFlushAsync();
}