using TicketHub.API.Extensions;
using TicketHub.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSwaggerServices();

builder.Services.AddCorsServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddCacheServices(builder.Configuration);

builder.Services.AddSignalR();


var app = builder.Build();

app.UseCors();

app.UseException();

await app.SeedDatabaseAsync();

app.UseLogRequest();

app.UseOutputCache();

app.UseSwagger(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.UseRedirection("/", "/swagger");

app.MapControllers();

app.MapHub<UserHub>("/hubs/user");

app.Run();

public partial class Program { }