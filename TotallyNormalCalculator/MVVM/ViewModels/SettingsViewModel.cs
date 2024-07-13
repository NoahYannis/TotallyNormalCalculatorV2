using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator.MVVM.ViewModels;

internal class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsRepository<SettingsModel> _settingsRepository;
    private readonly ITotallyNormalCalculatorLogger _logger;
    public SettingsViewModel(ISettingsRepository<SettingsModel> settingsRepository, ITotallyNormalCalculatorLogger logger)
    {
        (_settingsRepository, _logger) = (settingsRepository, logger);
    }
}
