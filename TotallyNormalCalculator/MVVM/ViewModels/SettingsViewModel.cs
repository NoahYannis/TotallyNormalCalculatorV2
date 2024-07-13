using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator.MVVM.ViewModels;

internal partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsRepository<SettingsModel> _settingsRepository;
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly  SettingsModel _userSetting;

    public SettingsViewModel(ISettingsRepository<SettingsModel> settingsRepository, ITotallyNormalCalculatorLogger logger)
    {
        (_settingsRepository, _logger) = (settingsRepository, logger);
        _userSetting = Task.Run(() => _settingsRepository.GetSettingAsync()).GetAwaiter().GetResult();
        UseDarkMode = _userSetting.DarkModeActive;
        SelectedLanguage = _userSetting.Language;
    }


    [ObservableProperty]
    private string _selectedLanguage;

    [ObservableProperty]
    private bool _useDarkMode;


    [RelayCommand]
    private async Task SaveSettings()
    {
        try
        {
            if (UseDarkMode == _userSetting.DarkModeActive && SelectedLanguage == _userSetting.Language)
                return; // No changes to save

            _userSetting.Language = SelectedLanguage;
            _userSetting.DarkModeActive = UseDarkMode;
            await _settingsRepository.UpdateSettingAsync(_userSetting);
        }
        catch (Exception exc)
        {
            _logger.LogExceptionToTempFile(exc);
        }
    }
}
