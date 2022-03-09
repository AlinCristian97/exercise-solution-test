namespace Core.Models;

public class MessageObj
{
    public string CatchKey { get; set; }
    public object Response { get; set; }
    public TimeSpan TimeToLive { get; set; }
}