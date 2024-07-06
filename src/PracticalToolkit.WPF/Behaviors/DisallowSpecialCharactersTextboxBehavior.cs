using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PracticalToolkit.WPF.Behaviors;

/// <summary>
///     A quick and convenient solution to prevent special characters
///     <br/>
///     such as
///     <see langword="\" /> <see langword="/" /> <see langword=":" /> <see langword="?" /> etc.
///     from being entered in text boxes.
///     <br />
///     Source
///     <see
///         href="https://stackoverflow.com/questions/24213964/whats-an-reusable-but-simple-and-efficient-way-to-disallow-special-characters" />
/// </summary>
public static class DisallowSpecialCharactersTextboxBehavior
{
    public static DependencyProperty DisallowSpecialCharactersProperty = DependencyProperty.RegisterAttached(
        "DisallowSpecialCharacters",
        typeof(bool),
        typeof(DisallowSpecialCharactersTextboxBehavior),
        new PropertyMetadata(DisallowSpecialCharactersChanged)
    );

    public static void SetDisallowSpecialCharacters(TextBox textBox, bool disallow)
    {
        textBox.SetValue(DisallowSpecialCharactersProperty, disallow);
    }

    public static bool GetDisallowSpecialCharacters(TextBox textBox)
    {
        return (bool)textBox.GetValue(DisallowSpecialCharactersProperty);
    }

    private static void DisallowSpecialCharactersChanged(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not TextBox tb) return;

        if ((bool)e.NewValue)
        {
            tb.PreviewTextInput += tb_PreviewTextInput;
            tb.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(tb_Pasting));
        }
        else
        {
            tb.PreviewTextInput -= tb_PreviewTextInput;
            tb.RemoveHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(tb_Pasting));
        }
    }

    private static void tb_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        var pastedText = e.DataObject.GetData(typeof(string))?.ToString() ?? "";

        Path.GetInvalidFileNameChars()
            .ToList()
            .ForEach(c =>
            {
                if (pastedText.Contains(c)) e.CancelCommand();
            });
    }

    private static void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (Path.GetInvalidFileNameChars().ToList().ConvertAll(x => x.ToString()).Contains(e.Text)) e.Handled = true;
    }
}