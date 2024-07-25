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

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumericUpDown),
            new PropertyMetadata(int.MaxValue));

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(NumericUpDown),
            new PropertyMetadata(12.0, null));


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

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        var decreaseBtn = (XamlIcon)Template.FindName("DecreaseBtn", this);
        var increaseBtn = (XamlIcon)Template.FindName("IncreaseBtn", this);

        decreaseBtn.MouseLeftButtonDown += DecreaseBtn_MouseLeftButtonDown;
        increaseBtn.MouseLeftButtonDown += IncreaseBtn_MouseLeftButtonDown;
    }

    private void DecreaseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

    private void IncreaseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!string.IsNullOrEmpty(Text) && int.TryParse(Text, out var intValue))
        {
            Text = intValue + Step > Maximum ? Maximum + "" : intValue + Step + "";
            SelectionStart = Text.Length;
        }
        else
        {
            Text = Minimum + "";
            SelectionStart = Text.Length;
        }
    }
}