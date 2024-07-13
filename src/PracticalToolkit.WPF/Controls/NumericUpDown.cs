using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PracticalToolkit.WPF.Controls;

public class NumericUpDown : TextBox
{
    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(int), typeof(NumericUpDown), new PropertyMetadata(1));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

    static NumericUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
            new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    public NumericUpDown()
    {
        InputMethod.SetIsInputMethodEnabled(this, false);
    }

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        var deButton = (XamlIcon)Template.FindName("DeButton", this);
        var inButton = (XamlIcon)Template.FindName("InButton", this);

        deButton.MouseLeftButtonDown += DeButton_MouseLeftButtonDown;
        inButton.MouseLeftButtonDown += InButton_MouseLeftButtonDown;
    }

    private void DeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!string.IsNullOrEmpty(Text))
        {
            Text = int.Parse(Text) - Step < Minimum ? Minimum + "" : int.Parse(Text) - Step + "";
            SelectionStart = Text.Length;
        }
        else
        {
            Text = Minimum + "";
            SelectionStart = Text.Length;
        }
    }

    private void InButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!string.IsNullOrEmpty(Text) && int.TryParse(Text, out var intValue))
        {
            Text = intValue + Step + "";
            SelectionStart = Text.Length;
        }
        else
        {
            Text = Minimum + "";
            SelectionStart = Text.Length;
        }
    }
}