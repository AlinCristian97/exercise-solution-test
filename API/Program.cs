using System.Text;
using Core.Interfaces;
using Infrastructure.Data.DbContexts;
using Infrastructure.Data.Repositories;
using Infrastructure.Data.SeedData;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IQuotesRepository, QuotesRepository>();
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"),
        true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<QuotesContext>(options => options.UseMySql(connectionString, 
    ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<QuotesContext>();
        await context.Database.MigrateAsync();
        await QuotesContextSeed.SeedAsync(context, loggerFactory);
    }
    catch (Exception e)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(e, "An error occured during migration");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();