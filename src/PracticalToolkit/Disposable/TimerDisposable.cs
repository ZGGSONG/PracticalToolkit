using System;
using System.Diagnostics;

namespace PracticalToolkit.Disposable;

/// <summary>
///     Represents a disposable timer that measures the elapsed time.
/// </summary>
public class TimerDisposable : IDisposable
{
    private readonly Action<TimeSpan>? _onElapsed;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimerDisposable" /> class.
    /// </summary>
    /// <param name="onElapsed">The action to be invoked when the timer is disposed.</param>
    public TimerDisposable(Action<TimeSpan>? onElapsed = null)
    {
        _onElapsed = onElapsed;
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    /// <summary>
    ///     Disposes the timer and stops the stopwatch.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes the timer and stops the stopwatch.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose() or from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources.
            _stopwatch.Stop();
            _onElapsed?.Invoke(_stopwatch.Elapsed);
        }

        // Dispose unmanaged resources here if any.

        _disposed = true;
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="TimerDisposable"/> class.
    /// </summary>
    ~TimerDisposable()
    {
        Dispose(false);
    }
}