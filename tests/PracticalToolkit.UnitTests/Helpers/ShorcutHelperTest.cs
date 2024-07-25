using PracticalToolkit.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PracticalToolkit.UnitTests.Helpers;

public class ShorcutHelperTest(ITestOutputHelper output)
{
    // Arrange
    private const string AppPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

    [Fact]
    public void TestShortcutStartup()
    {
        // Act
        Assert.True(ShortcutHelper.ShortcutCreate(AppPath));

        // Assert
        Assert.True(ShortcutHelper.ShortcutExist(AppPath));
        
        Thread.Sleep(1000);
        
        // Clean
        Assert.True(ShortcutHelper.ShortcutDelete(AppPath));
        
        // Assert
        Assert.False(ShortcutHelper.ShortcutExist(AppPath));
    }
    
    [Fact]
    public void TestShortcutCommonStartup()
    {
        // Act
        Assert.True(ShortcutHelper.ShortcutCreate(AppPath, Environment.SpecialFolder.CommonStartup));

        // Assert
        Assert.True(ShortcutHelper.ShortcutExist(AppPath, Environment.SpecialFolder.CommonStartup));
        
        Thread.Sleep(1000);
        
        // Clean
        Assert.True(ShortcutHelper.ShortcutDelete(AppPath, Environment.SpecialFolder.CommonStartup));
        
        // Assert
        Assert.False(ShortcutHelper.ShortcutExist(AppPath));
    }

    [Fact]
    public void TestShortcutDesktop()
    {
        var targetFolder = @"C:\";
        
        // Act
        Assert.True(ShortcutHelper.ShortcutCreate(AppPath, targetFolder));

        // Assert
        Assert.True(ShortcutHelper.ShortcutExist(AppPath, targetFolder));
        
        Thread.Sleep(1000);
        
        // Clean
        Assert.True(ShortcutHelper.ShortcutDelete(AppPath, targetFolder));
        
        // Assert
        Assert.False(ShortcutHelper.ShortcutExist(AppPath, targetFolder));
    }
}