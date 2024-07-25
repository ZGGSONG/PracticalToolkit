using System.ComponentModel;

namespace PracticalToolkit.WPF.Samples.Models;

public enum Fruit
{
    [Description("苹果")] Apple,

    [Description("香蕉")] Banana,

    [Description("梨子")] Pear,

    [Description("桃子")] Peach,

    [Description("水蜜桃")] HoneyPeach
}