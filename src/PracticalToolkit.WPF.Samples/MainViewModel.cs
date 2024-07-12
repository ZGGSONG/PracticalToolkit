using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticalToolkit.WPF.Samples.Models;
using System.Diagnostics;

namespace PracticalToolkit.WPF.Samples;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private Fruit _selectedFruit;

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            Debug.WriteLine($"Password Changed: {value}");
        }
    }

    [ObservableProperty]
    private string _content = DateTime.Now.ToString("HH:mm:ss.fff");

    [RelayCommand]
    private void Btn1()
    {
        Content = DateTime.Now.ToString("HH:mm:ss.fff");
    }

    [ObservableProperty]
    private string _passwordMarkText = string.Empty;
}