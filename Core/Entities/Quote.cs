using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Quote
{
    public int Id { get; set; }
    
    [MaxLength(500)]
    public string Text { get; set; }

    [MaxLength(100)]
    public string Author { get; set; }
}