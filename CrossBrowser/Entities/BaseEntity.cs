namespace CrossBrowser.Entities;

public abstract class BaseEntity
{
    public string Id { get; } = Guid.NewGuid().ToString();
}