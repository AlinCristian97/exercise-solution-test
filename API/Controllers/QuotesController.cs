using API.Helpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class QuotesController : BaseApiController
{
    private readonly IQuotesRepository _quotesRepository;

    public QuotesController(IQuotesRepository quotesRepository)
    {
        _quotesRepository = quotesRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Quote>>> GetQuotes()
    {
        IReadOnlyList<Quote> quotes = await _quotesRepository.GetQuotesAsync();

        return Ok(quotes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Quote>> GetQuote(int id)
    {
        Quote quote = await _quotesRepository.GetQuoteByIdAsync(id);

        if (quote == null) return NotFound();

        return Ok(quote);
    }

    [Cached(600)]
    [HttpGet("shout/{author}")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetXAmountOfShoutedQuotesTextsByAuthor(string author, [FromQuery] int? limit)
    {
        author = ProcessAuthor(author);
        
        limit ??= 10; //as 10 is the max allowed value, considering the exercise Constraints

        if (limit > 10)
        {
            return BadRequest("Limit cannot be greater than 10");
        }
        
        if (limit < 0)
        {
            return BadRequest("Limit cannot be a negative number");
        }

        IReadOnlyList<string> quotes = await _quotesRepository.GetXAmountOfShoutedQuotesTextsByAuthor(author, limit);
        
        return Ok(quotes);
    }

    #region Private Methods

    private string ProcessAuthor(string author)
    {
        string trimmedAuthor = author.Trim();
        
        if (trimmedAuthor.Contains('-'))
            trimmedAuthor = trimmedAuthor.Replace('-', ' ');

        return trimmedAuthor;
    }

    #endregion
}