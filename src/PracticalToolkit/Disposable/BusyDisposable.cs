namespace PracticalToolkit.Disposable;

public class BusyDisposable : IDisposable
{
    private readonly Action<bool> _busySetter;

    public BusyDisposable(Action<bool> busySetter)
    {
        _busySetter = busySetter;
        _busySetter(true);
    }

    public void Dispose()
    {
        _busySetter(false);
    }
}