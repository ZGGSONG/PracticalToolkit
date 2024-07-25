using System.Diagnostics;
using System.IO;
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
    [ObservableProperty] private BitmapSource? _bitmapSource2;

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
        Clear();
        using var runner = new ScreenshotRunner();
        using var bitmap = runner.Screenshot();
        BitmapSource = bitmap.ToBitmapSource();
    }

    [RelayCommand]
    private void ScreenshotAll()
    {
        Clear();
        using var runner = new ScreenshotRunner();
        using var bitmap = runner.ScreenshotAll();
        BitmapSource = bitmap.ToBitmapSource();
    }

    [RelayCommand]
    private void Save()
    {
        if (BitmapSource == null) return;
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Jpg|*.jpg|Png|*.png|All|*.*",
            FileName = $"{DateTime.Now:yyyyMMdd_HHmmss}"
        };
        if (saveFileDialog.ShowDialog() != true) return;
        var name = saveFileDialog.FileName;

        BitmapEncoder encoder = name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            ? new JpegBitmapEncoder()
            : new PngBitmapEncoder();

        // 将 BitmapSource 添加到 BitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(BitmapSource));

        // 使用 FileStream 保存到文件
        using FileStream fs = new(name, FileMode.Create);
        encoder.Save(fs);
    }

    [RelayCommand]
    private void Clear()
    {
        if (BitmapSource != null)
        {
            BitmapSource.Freeze();
            BitmapSource = null;
        }

        if (BitmapSource2 != null)
        {
            BitmapSource2.Freeze();
            BitmapSource2 = null;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [RelayCommand]
    private void Screenshoter()
    {
        Clear();
        using var screenshoter = new Screenshoter();
        using var bitmap = screenshoter.Screenshot();
        BitmapSource2 = bitmap.ToBitmapSource();
    }
}