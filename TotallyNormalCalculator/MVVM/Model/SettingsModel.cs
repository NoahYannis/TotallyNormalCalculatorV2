using System;

namespace TotallyNormalCalculator.MVVM.Model;

internal class SettingsModel : IModel
{
    public Guid Id { get; set; }
    public bool IsDarkMode { get; set; }
    public string Language { get; set; }
}
