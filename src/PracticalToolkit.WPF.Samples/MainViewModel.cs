using System.Diagnostics;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticalToolkit.WPF.Controls;
using PracticalToolkit.WPF.Samples.Models;
using PracticalToolkit.WPF.Utils;

namespace PracticalToolkit.WPF.Samples;

public partial class MainViewModel : ObservableObject
{
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
    private void ScreenShot()
    {
        var x = new ScreenShotCapture();
        x.OnCompleted += Completed;
        x.OnCanceled += Canceled;
        x.Capture();
    }

    private void Canceled()
    {
        Debug.WriteLine("取消截图");
    }

    private void Completed(BitmapSource bs)
    {
        bs.ToBitmap().Save(@"C:\Users\20230508\Desktop\Test.png");
    }
}