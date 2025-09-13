namespace Backend;

using System.Text.Json.Serialization;
using Core.Interfaces;
using Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Endpoints;
using Infrastructure.Data;
using Infrastructure.Parser;
using Infrastructure.Data.Uploads;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
        var uploadRelativePath = builder.Configuration["FileStorage:UploadPath"];
        if (string.IsNullOrEmpty(uploadRelativePath))
            throw new InvalidOperationException("Upload path is not configured in appsettings.json.");
        FilePathService.Init(builder.Environment.ContentRootPath, uploadRelativePath);
        UploadService.CleanTempFiles(FilePathService.UploadPath, TimeSpan.FromDays(1));
    
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
        builder.Services.AddScoped<ICsvDynamicParser, CsvDynamicParser>();
        builder.Services.AddSingleton<IParseResultCache, InMemoryParseResultCache>();

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddSingleton<ILoggingService, LoggingService>();


        var app = builder.Build();

        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI();


        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            db.Database.EnsureCreated();
        }

        app.MapEndpoints();
        app.Run("http://0.0.0.0:7108");
    }
}