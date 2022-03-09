using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class QuotesRepository : IQuotesRepository
{
    private readonly QuotesContext _context;

    public QuotesRepository(QuotesContext context)
    {
        _context = context;
    }
    
    public async Task<Quote> GetQuoteByIdAsync(int id)
    {
        Quote? quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id);

        return quote;
    }

    public async Task<IReadOnlyList<Quote>> GetQuotesAsync()
    {
        IReadOnlyList<Quote> quotes = await _context.Quotes.ToListAsync();

        return quotes;
    }

    public async Task<IReadOnlyList<string>> GetXAmountOfShoutedQuotesTextsByAuthor(string author, int? limit)
    {
        List<string> quotes = await _context.Quotes
            .Where(q => q.Author == author)
            .Take(limit.Value)
            .Select(q => q.Text
                .ToUpper()
                .Insert(q.Text.Length - 1, "!")
                .Remove(q.Text.Length))
            .ToListAsync();

        return quotes;
    }
}