
namespace WebBrowser.Models;

public class HistoryModel
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Host { get; set; }
    public string Query { get; set; }
    public DateTime LocalTimeStamp { get; set; }
}