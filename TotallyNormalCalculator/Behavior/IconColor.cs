using System.Windows;

namespace TotallyNormalCalculator.Behavior;

/// <summary>
/// An attached property to set the font awesome icon color on controls
/// </summary>
public static class IconColor
{
    public static readonly DependencyProperty IconColorProperty =
        DependencyProperty.RegisterAttached("IconColor", typeof(string), typeof(IconColor), new PropertyMetadata("Black"));

    public static void SetIconColor(UIElement element, string value)
    {
        element.SetValue(IconColorProperty, value);
    }

    public static string GetIconColor(UIElement element)
    {
        return (string)element.GetValue(IconColorProperty);
    }
}
