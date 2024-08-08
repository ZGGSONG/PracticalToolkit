using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PracticalToolkit.WPF.Helpers;

/// <summary>
///     文件扩展名助手类，提供文件扩展名关联、移除和检查功能
/// </summary>
public class FileExtHelper
{
    /// <summary>
    ///     关联文件扩展名
    /// </summary>
    /// <param name="extItem">扩展名</param>
    /// <param name="defineItem">定义项</param>
    /// <param name="appPath">应用程序路径</param>
    public static void AssociateFileExtension(string extItem, string defineItem, string appPath)
    {
        // 如果注册表中不存在该扩展名，则创建关联
        if (Registry.ClassesRoot.OpenSubKey(extItem) != null)
            return;
        // 创建扩展名关联项
        CreateSubKeyWithDefaultValue(extItem, defineItem);
        // 创建定义项
        CreateSubKeyWithDefaultValue(defineItem, null);
        // 创建打开命令项
        CreateSubKeyWithDefaultValue($"{defineItem}\\Shell\\Open\\Command", $"\"{appPath}\" \"%1\"");
        // 创建默认图标项
        CreateSubKeyWithDefaultValue($"{defineItem}\\DefaultIcon", $"{appPath},0");

        // 通知系统文件关联已更改
        NotifyFileAssociationChanged();
    }

    /// <summary>
    ///     移除文件扩展名关联
    /// </summary>
    /// <param name="extItem">扩展名</param>
    /// <param name="defineItem">定义项</param>
    public static void RemoveFileAssociation(string extItem, string defineItem)
    {
        // 删除扩展名关联项
        Registry.ClassesRoot.DeleteSubKeyTree(extItem, false);
        // 删除定义项
        Registry.ClassesRoot.DeleteSubKeyTree(defineItem, false);

        // 通知系统文件关联已更改
        NotifyFileAssociationChanged();
    }

    /// <summary>
    ///     检查文件扩展名是否已关联
    /// </summary>
    /// <param name="extItem">扩展名</param>
    /// <param name="defineItem">定义项</param>
    /// <returns>如果已关联则返回true，否则返回false</returns>
    public static bool CheckFileAssociation(string extItem, string defineItem)
    {
        using var key = Registry.ClassesRoot.OpenSubKey(extItem);
        if (key == null) return false;
        var value = key.GetValue(null) as string;
        return value == defineItem;
    }

    /// <summary>
    ///     创建注册表子项并设置默认值
    /// </summary>
    /// <param name="subKeyPath">子项路径</param>
    /// <param name="value">默认值</param>
    private static void CreateSubKeyWithDefaultValue(string subKeyPath, string? value)
    {
        using var subKey = Registry.ClassesRoot.CreateSubKey(subKeyPath);
        if (value != null && subKey != null) subKey.SetValue(null, value);
    }

    /// <summary>
    ///     通知系统文件关联已更改
    /// </summary>
    private static void NotifyFileAssociationChanged()
    {
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }

    #region Import

    // 导入shell32.dll中的SHChangeNotify函数，用于通知系统文件关联已更改
    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    // 文件关联更改事件ID
    private const uint SHCNE_ASSOCCHANGED = 0x08000000;

    // 通知标志
    private const uint SHCNF_IDLIST = 0x0000;

    #endregion
}