using System.Windows;

namespace PracticalToolkit.WPF.Utils;

/// <summary>
///     This is primarily used on controls that cannot inherit DataContext,
///     such as Popup, ContextMenu, and Tooltip, to bind to a DataContext.
///     <br />
///     Source:
///     <see href="https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/" />
/// </summary>
public sealed class BindingProxy : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(object),
        typeof(BindingProxy),
        new PropertyMetadata(default(object))
    );

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    public override string ToString()
    {
        return Data is FrameworkElement fe
            ? $"{nameof(BindingProxy)}: {fe.Name}"
            : $"{nameof(BindingProxy)}: {Data?.GetType().FullName}";
    }
}