namespace WebBrowser.Entities;

public abstract class BaseEntity
{
    public string Id { get; } = Guid.NewGuid().ToString();
}