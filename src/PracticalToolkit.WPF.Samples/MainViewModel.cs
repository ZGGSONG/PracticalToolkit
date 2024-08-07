﻿using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PracticalToolkit.Screenshot;
using PracticalToolkit.WPF.Helpers;
using PracticalToolkit.WPF.Samples.Models;

namespace PracticalToolkit.WPF.Samples;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private bool _isDrawBorder;
    [ObservableProperty] private bool _isDrawMagnifier;
    [ObservableProperty] private double _opacity = 0.1;
    [ObservableProperty] private Color _borderColor = Color.Red;
    
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
        Clear();
        var options = new RunnerOptions(IsDrawBorder, IsDrawMagnifier, Opacity, BorderColor);
        using var runner = new ScreenshotRunner(options);
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

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }


    [RelayCommand]
    private void AddAssociation()
    {
        if (!ProgressHelper.IsRunAsAdmin())
        {
            ProgressHelper.ReStartAsAdmin();
            return;
        }
        var path = Process.GetCurrentProcess().MainModule?.FileName ?? "";
        FileExtHelper.AssociateFileExtension(".practicaltoolkit", "practicaltoolkit.wpf.sample", path);
    }
    
    [RelayCommand]
    private void RemoveAssociation()
    {
        if (!ProgressHelper.IsRunAsAdmin())
        {
            ProgressHelper.ReStartAsAdmin();
            return;
        }
        FileExtHelper.RemoveFileAssociation(".practicaltoolkit", "practicaltoolkit.wpf.sample");
    }
    
    [RelayCommand]
    private void ExistAssociation()
    {
        var ret = FileExtHelper.CheckFileAssociation(".practicaltoolkit", "practicaltoolkit.wpf.sample");
        MessageBox.Show($"check result: {ret}");
    }
}