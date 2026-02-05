using System.Net;
using TicketHub.API.Models;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.API.Extensions
{
    public class LogRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public LogRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

            await _next.Invoke(context);

            logger.LogInformation($"Response: {context.Response.StatusCode}");
        }
    }

    public static class LogRequestMiddlewareExtension
    {
        public static IApplicationBuilder UseLogRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogRequestMiddleware>();
        }
    }

    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _fromPath;
        private readonly string _toPath;

        public RedirectMiddleware(RequestDelegate next, string fromPath, string toPath)
        {
            _next = next;
            _fromPath = fromPath;
            _toPath = toPath;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == _fromPath)
            {
                context.Response.Redirect(_toPath);
                return;
            }

            await _next.Invoke(context);
        }
    }

    public static class RedirectMiddlewareExtension
    {
        public static IApplicationBuilder UseRedirection(this IApplicationBuilder builder, string fromPath, string toPath)
        {
            return builder.UseMiddleware<RedirectMiddleware>(fromPath, toPath);
        }
    }

    public static class DbInitializerExtension
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                    string[] roles = ["Admin", "User"];
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                            await roleManager.CreateAsync(new IdentityRole(role));
                    }

                    var adminEmail = "admin@localhost.com";
                    var adminUser = await userManager.FindByEmailAsync(adminEmail);

                    if (adminUser == null)
                    {
                        var newAdmin = new ApplicationUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(newAdmin, "@Admin123");

                        if (result.Succeeded)
                            await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al realizar el seeding de datos.");
                }
            }
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new { StatusCode = 500, Message = "Ocurrió un error interno", Detailed = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    public static class ExceptionMiddlewareExtension
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>(
                builder.ApplicationServices.GetRequiredService<ILogger<ExceptionMiddleware>>()
                );
        }
    }

    public static class SwaggerMiddleware
    {
        public static WebApplication UseSwagger(this WebApplication application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            return application;
        }
    }
}