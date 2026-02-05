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


var app = builder.Build();

app.UseException();

await app.SeedDatabaseAsync();

app.UseLogRequest();

app.UseCors();

app.UseOutputCache();

app.UseSwagger(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.UseRedirection("/", "/swagger");

app.MapControllers();

app.Run();

public partial class Program { }