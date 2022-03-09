using Core.Entities;

namespace Core.Interfaces;

public interface IQuotesRepository
{
    Task<Quote> GetQuoteByIdAsync(int id);
    Task<IReadOnlyList<Quote>> GetQuotesAsync();
    Task<IReadOnlyList<string>> GetXAmountOfShoutedQuotesTextsByAuthor(string author, int? limit);
}