using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PracticalToolkit.WPF.Controls;

/// <summary>
///     带占位符的文本输入控件
/// </summary>
public class PlaceholderTextBox : TextBox
{
    #region Public Methods

    public PlaceholderTextBox()
    {
        var placeholderBinding = new Binding { Source = this, Path = new PropertyPath(nameof(Placeholder)) };
        _placeholderTextBlock.SetBinding(TextProperty, placeholderBinding);
        _placeholderTextBlock.FontWeight = FontWeights.Thin;
        _placeholderTextBlock.Padding = new Thickness(3, 0, 3, 0);
        _placeholderTextBlock.BorderThickness = new Thickness(0);
        _placeholderTextBlock.Background = Brushes.Transparent;
        _placeholderTextBlock.Foreground = Brushes.Gray;

        _placeholderVisualBrush.AlignmentX = AlignmentX.Left;
        _placeholderVisualBrush.AlignmentY = AlignmentY.Center;
        _placeholderVisualBrush.Stretch = Stretch.None;
        _placeholderVisualBrush.Visual = _placeholderTextBlock;

        Background = _placeholderVisualBrush;
        TextChanged += PlaceholderTextBox_TextChanged;
    }

    #endregion Public Methods

    #region Events Handling

    /// <summary>
    ///     Response to text changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaceholderTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Background = string.IsNullOrEmpty(Text) ? _placeholderVisualBrush : null;
    }

    #endregion Events Handling

    #region Fields

    /// <summary>
    ///     Text box with placeholder text.
    /// </summary>
    private readonly TextBox _placeholderTextBlock = new();

    /// <summary>
    ///     Placeholder brush
    /// </summary>
    private readonly VisualBrush _placeholderVisualBrush = new();

    #endregion Fields

    #region Properties

    /// <summary>
    ///     Placeholder Dependency Properties
    /// </summary>
    public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
        nameof(Placeholder),
        typeof(string),
        typeof(PlaceholderTextBox),
        new FrameworkPropertyMetadata("Please Input...", FrameworkPropertyMetadataOptions.AffectsRender)
    );

    /// <summary>
    ///     占位符
    /// </summary>
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    #endregion Properties
}