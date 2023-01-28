
using kataviv.Features.Ads;
using kataviv.Features.Weathers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kataviv;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddHttpClient();

        builder.Services.AddDbContext<AdDbContext>(options =>
        {
            var local = Environment.SpecialFolder.LocalApplicationData;
            var folder = Environment.GetFolderPath(local);
            var path = Path.Join(folder, "db.sqlite");
            options.UseSqlite($"Data Source={path}");
        });
        builder.Services.AddTransient<AdRepository>((services) => new AdEfRepository(services.GetRequiredService<AdDbContext>()));
        builder.Services.AddTransient<WeatherService>((services) =>
        {
            if (Environment.GetEnvironmentVariable("Testing") is null)
            {
                return new OpenMeteoWeatherService(services.GetRequiredService<IHttpClientFactory>());
            }

            return new FakeWeatherService();
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        using var scope = app.Services.CreateScope();
        {
            var db = scope.ServiceProvider.GetRequiredService<AdDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        app.Run();
    }
}
