namespace CrossBrowser.Entities;

public class HistoryEntity : BaseEntity
{
    public string Url { get; set; }
    public string Query { get; set; }
    public DateTime UtcTimeStamp { get; set; }
}