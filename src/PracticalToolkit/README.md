# Application Method

## Disposable

### BusyDisposable

1. The first way to use it.

```csharp
public async Task DoSomething()
{
    using var _ = new BusyDisposable(value => IsBusy = value);
    
    // Simulate a time-consuming operation.
    await Task.Delay(3000);
}
```

2. The second way to use it.

```csharp
private BusyDisposable NewBusyDisposable => new(value => IsBusy = value);

public async Task DoSomething()
{
    using (var _ = NewBusyDisposable)
    {
        // Simulate a time-consuming operation.
        await Task.Delay(3000);
    }
}
```

## TimerDisposable

```csharp
public void DoSomething()
{
    using var _ = new TimerDisposable(elapsed =>
    {
        System.Diagnostics.Debug.WriteLine($"操作耗时: {elapsed}");
    });
    // Perform operations that require timing here.
    Thread.Sleep(_timeOutSecond);
}
```