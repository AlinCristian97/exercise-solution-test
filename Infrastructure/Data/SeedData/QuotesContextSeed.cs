using System.Text.Json;
using Core.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.SeedData;

public class QuotesContextSeed
{
    public static async Task SeedAsync(QuotesContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            if (!context.Quotes.Any())
            {
                string quotesData = File.ReadAllText("../Infrastructure/Data/SeedData/quotes.json");
                
                List<Quote>? quotes = JsonSerializer.Deserialize<List<Quote>>(quotesData,
                    new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});

                foreach (Quote quote in quotes)
                {
                    context.Quotes.Add(quote);
                }

                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            var logger = loggerFactory.CreateLogger<QuotesContextSeed>();
            logger.LogError(e.Message);
        }
    }
}