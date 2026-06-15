using Serilog;
using DataIntegrationHub.Data;
using Microsoft.EntityFrameworkCore;
using DataIntegrationHub.Settings;
using DataIntegrationHub.Interfaces;
using DataIntegrationHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DataIntegrationHub;

public class Program
{

    public static void Main(string[] args)
    {
        // 1. Bootstrap Serilog configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting Data Integration Hub API...");

            var builder = WebApplication.CreateBuilder(args);

            // 1. Mapeamos la configuración de JWT (Options Pattern)
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection(JwtSettings.SectionName));

            // 2. Registramos nuestro servicio generador de tokens
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // 3. Configuramos la validación del Token
            builder.Services.AddAuthentication(options =>
            {
                // Establecemos JWT como el esquema por defecto para evitar tener que especificarlo en cada endpoint
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

                // Si jwtSettings es null (problema de lectura), lanzamos excepción para que la API no arranque de forma insegura
                ArgumentNullException.ThrowIfNull(jwtSettings);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            // Agregamos el servicio de autorización
            builder.Services.AddAuthorization();

            builder.Host.UseSerilog();
            

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            

            // 4. Add services to the container
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();



            // 4. Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Primero validamos QUIÉN es el usuario y si su token es válido (Authentication)
            app.UseAuthentication();

            // Después validamos QUÉ tiene permitido hacer según su rol (Authorization)
            app.UseAuthorization();

            app.MapControllers();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex) when (ex is not HostAbortedException) // <- AGREGAMOS ESTA CONDICIÓN
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}