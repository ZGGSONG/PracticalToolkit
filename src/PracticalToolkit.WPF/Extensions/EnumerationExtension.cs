using System.ComponentModel;
using System.Windows.Markup;

namespace PracticalToolkit.WPF.Extensions;

/// <summary>
///     Assist in quickly binding enum types to ComboBox.
///     <see href="https://blog.csdn.net/kristen_dou/article/details/133675830" />
/// </summary>
public class EnumerationExtension : MarkupExtension
{
    private Type _enumType = null!;

    public EnumerationExtension(Type enumType)
    {
        EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
    }

    public Type EnumType
    {
        get => _enumType;
        set
        {
            if (_enumType == value) return;

            var enumType = Nullable.GetUnderlyingType(value) ?? value;
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be an Enum.");

            _enumType = value;
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var enumValues = Enum.GetValues(EnumType!);
        return (from object enumValue in enumValues
            select new EnumerationMember { Value = enumValue, Description = GetDescription(enumValue) }).ToArray();
    }

    private string GetDescription(object enumValue)
    {
        var defaultValue = enumValue.ToString() ?? string.Empty;
        var fieldInfo = EnumType.GetField(defaultValue);
        if (fieldInfo == null) return defaultValue;

        var obj = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
        return obj is not DescriptionAttribute descriptionAttribute ? defaultValue : descriptionAttribute.Description;
    }

    public class EnumerationMember
    {
        public string Description { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}