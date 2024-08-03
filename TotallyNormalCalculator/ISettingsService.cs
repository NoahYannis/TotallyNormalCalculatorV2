using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator;
public interface ISettingsService
{
    void ApplySettings(SettingsModel settings);
}