using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PracticalToolkit.Screenshot;
using PracticalToolkit.WPF.Samples.Models;

namespace PracticalToolkit.WPF.Samples;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private BitmapSource? _bitmapSource;

    [ObservableProperty] private string _content = DateTime.Now.ToString("HH:mm:ss.fff");

    private string _password = string.Empty;

    [ObservableProperty] private string _passwordMarkText = string.Empty;

    [ObservableProperty] private Fruit _selectedFruit;

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            Debug.WriteLine($"Password Changed: {value}");
        }
    }

    [RelayCommand]
    private void Btn1()
    {
        Content = DateTime.Now.ToString("HH:mm:ss.fff");
    }

    [RelayCommand]
    private void Screenshot()
    {
        var runner = new ScreenshotRunner();
        using var bitmap = runner.Screenshot();
        BitmapSource = bitmap.ToBitmapSource();
    }

    [RelayCommand]
    private void ScreenshotAll()
    {
        var runner = new ScreenshotRunner();
        using var bitmap = runner.ScreenshotAll();
        BitmapSource = bitmap.ToBitmapSource();
    }

    [RelayCommand]
    private void Save()
    {
        using var bitmap = BitmapSource.ToBitmap();
        if (bitmap == null) return;
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Png|*.png|All|*.*",
            FileName = $"{DateTime.Now:yyyyMMdd_HHmmss}"
        };
        if (saveFileDialog.ShowDialog() != true) return;
        // 确保文件路径有效且可写
        var name = saveFileDialog.FileName;
        var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
        using var graphics = Graphics.FromImage(newBitmap);
        graphics.DrawImage(bitmap, 0, 0);
        newBitmap.Save(name);
    }
}