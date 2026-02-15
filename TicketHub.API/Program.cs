using Serilog;
using TicketHub.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSwaggerServices();

builder.Services.AddCorsServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddCacheServices(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});


var app = builder.Build();

app.UseCors();

app.UseException();

await app.SeedDatabaseAsync();

app.UseSerilogRequestLogging();

app.UseOutputCache();

app.UseSwagger(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.UseRedirection("/", "/swagger");

app.MapControllers();

app.Run();

public partial class Program { }