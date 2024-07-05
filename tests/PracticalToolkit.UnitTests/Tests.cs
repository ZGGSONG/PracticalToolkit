using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PracticalToolkit.UnitTests;

public class Tests
{
    [Fact()]
    public async Task AsyncTest()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var cancellationToken = cancellationTokenSource.Token;

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    [Fact()]
    public void Test()
    {
        Assert.Equal("", "");
    }
}