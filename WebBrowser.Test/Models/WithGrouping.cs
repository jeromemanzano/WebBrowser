using WebBrowser.Models;

namespace WebBrowser.Test.Models;

public class WithGrouping
{
    [Test]
    public void Create_Should_Throw_ArgumentNullException_When_Group_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new Grouping<string, object>(default));
    }
}