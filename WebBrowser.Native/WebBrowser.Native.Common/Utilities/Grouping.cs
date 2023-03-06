using DynamicData;
using DynamicData.Binding;

namespace WebBrowser.Native.Common.Utilities;

public class Grouping<TKey, TObject> : ObservableCollectionExtended<TObject>, IGrouping<TKey, TObject>, IDisposable
{
    private readonly IDisposable? _groupDisposable;
    public TKey Key { get; }

    public Grouping(IGroup<TObject, TKey>? group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        Key = group.GroupKey;
        _groupDisposable = group
            .List
            .Connect()
            .Bind(this)
            .Subscribe();
    }

    public void Dispose()
    {
        _groupDisposable?.Dispose();
    }
}
