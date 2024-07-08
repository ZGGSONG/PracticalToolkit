using PracticalToolkit.Disposable;
using Xunit;
using Xunit.Abstractions;

namespace PracticalToolkit.UnitTests.Disposable;

public class TimerDisposableTests(ITestOutputHelper testOutputHelper)
{
    private const double Tolerance = 30.0;
    private readonly TimeSpan _timeOutCancelSecond = TimeSpan.FromSeconds(0.5);
    private readonly TimeSpan _timeOutSecond = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task AsyncTest()
    {
        using var _ = new TimerDisposable(elapsed =>
        {
            Assert.Equal(elapsed.TotalMilliseconds, _timeOutSecond.TotalMilliseconds, Tolerance);
            testOutputHelper.WriteLine($"操作耗时: {elapsed}\t预计时间: {_timeOutSecond}\t误差时长: {Tolerance}ms");
        });
        await Task.Delay(_timeOutSecond);
    }

    [Fact]
    public async Task AsyncWithCancelTest()
    {
        using var cancellationTokenSource = new CancellationTokenSource(_timeOutCancelSecond);
        var cancellationToken = cancellationTokenSource.Token;

        using var _ = new TimerDisposable(elapsed =>
        {
            Assert.Equal(elapsed.TotalMilliseconds, _timeOutCancelSecond.TotalMilliseconds,
#if NET8_0
                Tolerance
#else
                80.0
#endif
            );
            testOutputHelper.WriteLine("操作耗时: {0}\t预计时间: {1}\t误差时长: {2}ms", elapsed, _timeOutCancelSecond,
#if NET8_0
                Tolerance
#else
                80.0
#endif
            );
        });
        try
        {
            await Task.Delay(_timeOutSecond, cancellationToken);
        }
        catch (Exception e)
        {
            testOutputHelper.WriteLine(e.ToString());
        }
    }

    [Fact]
    public void Test()
    {
        using var _ = new TimerDisposable(elapsed =>
        {
            Assert.Equal(elapsed.TotalMilliseconds, _timeOutSecond.TotalMilliseconds, Tolerance);
            testOutputHelper.WriteLine($"操作耗时: {elapsed}\t预计时间: {_timeOutSecond}\t误差时长: {Tolerance}ms");
        });
        // 在这里执行需要计时的操作
        Thread.Sleep(_timeOutSecond);
    }
}