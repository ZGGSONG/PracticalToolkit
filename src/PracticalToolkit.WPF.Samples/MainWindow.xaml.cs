using System.Windows.Input;

namespace PracticalToolkit.WPF.Samples;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Control_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (ImageScaleTransform == null) return;
        var zoom = e.Delta > 0 ? 0.1 : -0.1;
        ImageScaleTransform.ScaleX += zoom;
        ImageScaleTransform.ScaleY += zoom;

        // 限制缩放比例
        if (ImageScaleTransform.ScaleX < 0.1) ImageScaleTransform.ScaleX = 0.1;
        if (ImageScaleTransform.ScaleY < 0.1) ImageScaleTransform.ScaleY = 0.1;
    }
}