using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PracticalToolkit.Helpers;

/// <summary>
///     提供创建、检查和删除快捷方式的功能。
/// </summary>
public class ShortcutHelper
{
    /// <summary>
    ///     在指定的系统特殊文件夹中创建快捷方式。
    /// </summary>
    /// <param name="path">快捷方式指向的目标文件的绝对路径。</param>
    /// <param name="folder">系统特殊文件夹，默认为启动文件夹。</param>
    /// <returns>如果创建成功返回true，否则返回false。</returns>
    public static bool ShortcutCreate(string path, Environment.SpecialFolder folder = Environment.SpecialFolder.Startup)
    {
        return ShortcutCreate(path, Environment.GetFolderPath(folder));
    }

    /// <summary>
    ///     检查在指定的系统特殊文件夹中是否存在快捷方式。
    /// </summary>
    /// <param name="path">快捷方式指向的目标文件的绝对路径。</param>
    /// <param name="folder">系统特殊文件夹，默认为启动文件夹。</param>
    /// <returns>如果快捷方式存在返回true，否则返回false。</returns>
    public static bool ShortcutExist(string path, Environment.SpecialFolder folder = Environment.SpecialFolder.Startup)
    {
        return ShortcutExist(path, Environment.GetFolderPath(folder));
    }

    /// <summary>
    ///     检查在指定的系统特殊文件夹中是否存在快捷方式。
    /// </summary>
    /// <param name="path">快捷方式指向的目标文件的绝对路径。</param>
    /// <param name="folder">系统特殊文件夹，默认为启动文件夹。</param>
    /// <returns>如果删除成功返回true，否则返回false。</returns>
    public static bool ShortcutDelete(string path, Environment.SpecialFolder folder = Environment.SpecialFolder.Startup)
    {
        return ShortcutDelete(path, Environment.GetFolderPath(folder));
    }

    /// <summary>
    ///     创建快捷方式
    /// </summary>
    /// <param name="path">快捷方式目标（可执行文件的绝对路径）</param>
    /// <param name="target">目标文件夹（绝对路径）</param>
    /// <returns>如果创建成功返回true，否则返回false。</returns>
    public static bool ShortcutCreate(string path, string target)
    {
        var result = true;
        try
        {
            var lnkPath = Path.Combine(target, Path.GetFileNameWithoutExtension(path) + ".lnk");
            ShortcutCreateInternal(path, lnkPath);
        }
        catch
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    ///     判断快捷方式是否存在
    /// </summary>
    /// <param name="path">快捷方式目标（可执行文件的绝对路径）</param>
    /// <param name="target">目标文件夹（绝对路径）</param>
    /// <returns>如果快捷方式存在返回true，否则返回false。</returns>
    public static bool ShortcutExist(string path, string target)
    {
        var result = false;
        var list = GetDirectoryFileList(target);
        foreach (var item in list.Where(item => path == GetAppPathViaShortCut(item))) result = true;
        return result;
    }

    /// <summary>
    ///     删除快捷方式
    /// </summary>
    /// <param name="path">快捷方式目标（可执行文件的绝对路径）</param>
    /// <param name="target">目标文件夹（绝对路径）</param>
    /// <returns>如果删除成功返回true，否则返回false。</returns>
    public static bool ShortcutDelete(string path, string target)
    {
        var result = false;
        var list = GetDirectoryFileList(target);
        foreach (var item in list.Where(item => path == GetAppPathViaShortCut(item)))
        {
            File.Delete(item);
            result = true;
        }

        return result;
    }

    /// <summary>
    ///     获取指定文件夹下的所有快捷方式（不包括子文件夹）
    /// </summary>
    /// <param name="target">目标文件夹（绝对路径）</param>
    /// <returns></returns>
    private static List<string> GetDirectoryFileList(string target)
    {
        List<string> list = [];
        list.Clear();
        var files = Directory.GetFiles(target, "*.lnk");
        if (files.Length == 0) return list;

        list.AddRange(files);
        return list;
    }

    #region 非 COM 实现快捷键创建

    /// <summary>
    ///     向目标路径创建指定文件的快捷方式
    /// </summary>
    /// <param name="appPath">App路径</param>
    /// <param name="shortcutPath">快捷方式路径</param>
    /// <param name="description">提示信息</param>
    private static void ShortcutCreateInternal(string appPath, string shortcutPath, string description = "")
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        var link = (IShellLink)new ShellLink();
        if (!string.IsNullOrEmpty(description))
            link.SetDescription(description);
        link.SetPath(appPath);

        if (File.Exists(shortcutPath))
            File.Delete(shortcutPath);
        link.Save(shortcutPath, false);
    }

    /// <see href="https://blog.csdn.net/weixin_42288222/article/details/124150046" />
    /// <summary>
    ///     获取快捷方式中的目标（可执行文件的绝对路径）
    /// </summary>
    /// <param name="shortCutPath">快捷方式的绝对路径</param>
    /// <returns></returns>
    private static string? GetAppPathViaShortCut(string shortCutPath)
    {
        try
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var file = (IShellLink)new ShellLink();
            file.Load(shortCutPath, 2);
            var sb = new StringBuilder(256);
            file.GetPath(sb, sb.Capacity, IntPtr.Zero, 2);
            return sb.ToString();
        }
        catch
        {
            return null;
        }
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink : IPersistFile
    {
        void GetPath([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd,
            int fFlags);

        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);

        void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath,
            out int piIcon);

        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    #endregion
}