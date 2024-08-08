using System.Diagnostics;
using System.Security.Principal;

namespace PracticalToolkit.WPF.Helpers;

/// <summary>
///     进程助手类，提供与进程相关的功能
/// </summary>
public static class ProgressHelper
{
    /// <summary>
    ///     检查当前进程是否以管理员身份运行
    /// </summary>
    /// <returns>如果以管理员身份运行则返回true，否则返回false</returns>
    public static bool IsRunAsAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     以管理员身份重新启动当前进程
    /// </summary>
    /// <returns>如果成功启动则返回true，否则返回false</returns>
    public static bool ReStartAsAdmin()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Process.GetCurrentProcess().MainModule!.FileName,
                Verb = "runas"
            };
            Process.Start(startInfo);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            Environment.Exit(0);
        }
    }

    /// <summary>
    ///     检查指定名称的进程是否正在运行
    /// </summary>
    /// <param name="processName">进程名称</param>
    /// <returns>如果进程正在运行则返回true，否则返回false</returns>
    public static bool IsProcessRunning(string processName)
    {
        try
        {
            return Process.GetProcesses().Any(process =>
                process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     终止指定名称的进程
    /// </summary>
    /// <param name="processName">进程名称</param>
    public static void TerminateProcess(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        foreach (var process in processes)
        {
            try
            {
                if (!process.HasExited) process.Kill();
            }
            catch
            {
                // 忽略异常
            }
        }
    }
}