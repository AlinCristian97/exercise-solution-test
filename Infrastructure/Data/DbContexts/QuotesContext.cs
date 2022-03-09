using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContexts;

public class QuotesContext : DbContext
{
    public QuotesContext(DbContextOptions<QuotesContext> options) : base(options)
    {
    }

    public DbSet<Quote> Quotes { get; set; }
}