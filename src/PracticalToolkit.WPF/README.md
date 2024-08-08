# Application Method

Introducing Namespaces

```xaml
xmlns:s="https://github.com/zggsong/2022/xaml"
```

```xml
<ResourceDictionary Source="pack://application:,,,/PracticalToolkit.WPF;component/Themes/Generic.xaml" />
```

# Useage

- View the [Samples](https://github.com/ZGGSONG/PracticalToolkit/tree/main/src/PracticalToolkit.WPF.Samples) Project
- Below is the example code.

## EnumerationExtension

> Assist in quickly binding enum types to ComboBox.

```xaml
<ComboBox Margin="0 5"
	ItemsSource="{Binding Source={s:Enumeration {x:Type  model:Fruit}}}"
	SelectedValue="{Binding SelectedFruit, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
	DisplayMemberPath="Description"
	SelectedValuePath="Value" />
```

- ViewModel

```csharp
private string _password = string.Empty;
public string Password
{
    get => _password;
    set => SetProperty(ref _password, value);
}
```

- Model

```csharp
public enum Fruit
{
    [Description("苹果")]
    Apple,

    [Description("香蕉")]
    Banana,

    [Description("梨子")]
    Pear,

    [Description("桃子")]
    Peach,
}
```

## BindingProxy

> This is primarily used on controls that cannot inherit DataContext, such as Popup, ContextMenu, and Tooltip, to bind
> to a DataContext.

```xaml
<Window.Resources>
    <s:BindingProxy x:Key="Vm" Data="{Binding}" />
</Window.Resources>
```

```xaml
<ToggleButton Margin="0 5"
    Name="ModeTb"
    Content="点击展开" />
<Popup IsOpen="{Binding ElementName=ModeTb, Path=IsChecked}"
    StaysOpen="False"
    AllowsTransparency="True"
    PopupAnimation="Slide"
    PlacementTarget="{Binding ElementName=ModeTb}"
    Placement="Bottom">
    <Border CornerRadius="5"
            MinWidth="80"
            Background="#202020"
            BorderBrush="#6ec2d2"
            BorderThickness="1">
        <StackPanel Background="Transparent" HorizontalAlignment="Center">
            <Button Content="刷新时间1"
                    Margin="5"
                    Command="{Binding Source={StaticResource Vm}, Path=Data.Btn1Command}" />
            <TextBlock Text="{Binding Source={StaticResource Vm}, Path=Data.Content}"
                        Margin="5" />
            <Button Content="刷新时间2"
                    Margin="5"
                    Command="{Binding Source={StaticResource Vm}, Path=Data.Btn1Command}" />
            <Separator />
        </StackPanel>
    </Border>
</Popup>
```

## DisallowSpecialCharactersTextboxBehavior

> Quickly and easily disable special characters (i.e. "\ / : ? " < > |") in these text boxes.

```xaml
<TextBox Margin="5" s:DisallowSpecialCharactersTextboxBehavior.DisallowSpecialCharacters="True" />
```

## PasswordHelper

> Databind the Password Property of a WPF PasswordBox

```xaml
<PasswordBox Margin="0 5"
    s:PasswordHelper.Attach="True"
    s:PasswordHelper.Password="{Binding Password, Mode=TwoWay}" />
```

## PlaceholderTextBox

> A text input control with placeholders.

```xaml
<s:PlaceholderTextBox Placeholder="Username" />
```

## PasswordBoxMarkTextStyle

> Support password box to display binding watermark information by Tag
> Required to work with `PasswordHelper`

```xaml
<PasswordBox Margin="0 5"
    Style="{DynamicResource PasswordBoxMarkTextStyle}"
    Tag="输入你的密码"
    s:PasswordHelper.Attach="True"
    s:PasswordHelper.Password="{Binding PasswordMarkText, Mode=TwoWay}" />
```

## TextBoxRoundedStyle

> Rounded text box style

```xaml
<TextBox Margin="0 5" Style="{DynamicResource TextBoxRoundedStyle}" />
```

## NumericUpDown

```xaml
<s:NumericUpDown Minimum="0"
    Maximum="50"
    Step="5"
    FontSize="14"
    IconSize="14"
    Tag="请输入数字..." />
```

## ProgressHelper

```csharp
if (!ProgressHelper.IsRunAsAdmin())
{
    ProgressHelper.ReStartAsAdmin();
    return;
}
```

## FileExtHelper

```csharp
var path = Process.GetCurrentProcess().MainModule.FileName;
FileExtHelper.AssociateFileExtension(".practicaltoolkit", "practicaltoolkit.wpf.sample", path);
FileExtHelper.RemoveFileAssociation(".practicaltoolkit", "practicaltoolkit.wpf.sample");
FileExtHelper.CheckFileAssociation(".practicaltoolkit", "practicaltoolkit.wpf.sample");
```