using PracticalToolkit.Disposable;
using Xunit;
using Xunit.Abstractions;

namespace PracticalToolkit.UnitTests.Disposable;

/// <summary>
///     <see href="https://blog.coldwind.top/posts/mimic-go-defer-in-csharp/" />
/// </summary>
/// <param name="testOutputHelper"></param>
public class BusyDisposableTests(ITestOutputHelper testOutputHelper)
{
    private bool IsBusy { get; set; }

    [Fact]
    public async Task Busy1Test()
    {
        testOutputHelper.WriteLine($"<Begin> {IsBusy}");
        Assert.False(IsBusy);
        using (var _ = new BusyDisposable(value => IsBusy = value))
        {
            testOutputHelper.WriteLine($"<Execute> {IsBusy}");

            Assert.True(IsBusy);
            // Simulate a time-consuming operation.
            await Task.Delay(3000);
            Assert.True(IsBusy);
        }

        testOutputHelper.WriteLine($"<End> {IsBusy}");
        Assert.False(IsBusy);
    }

    #region Another Sample

    private BusyDisposable NewBusyDisposable => new(value => IsBusy = value);

    [Fact]
    public async Task Busy2Test()
    {
        testOutputHelper.WriteLine($"<Begin> {IsBusy}");
        Assert.False(IsBusy);
        using (var _ = NewBusyDisposable)
        {
            testOutputHelper.WriteLine($"<Execute> {IsBusy}");

            Assert.True(IsBusy);
            // Simulate a time-consuming operation.
            await Task.Delay(3000);
            Assert.True(IsBusy);
        }

        testOutputHelper.WriteLine($"<End> {IsBusy}");
        Assert.False(IsBusy);
    }

    #endregion
}