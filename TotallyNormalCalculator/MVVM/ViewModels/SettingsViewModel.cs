using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using TotallyNormalCalculator.Languages;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator.MVVM.ViewModels;

internal partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsRepository<SettingsModel> _settingsRepository;
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly SettingsModel _userSettings;
    private readonly SettingsService _settingsService;

    public SettingsViewModel
        (ISettingsRepository<SettingsModel> settingsRepository,
        ITotallyNormalCalculatorLogger logger,
        SettingsService settingsService)
    {
        (_settingsRepository, _logger, _settingsService) = (settingsRepository, logger, settingsService);
        _userSettings = Task.Run(() => _settingsRepository.GetUserSettings()).GetAwaiter().GetResult();
        UseDarkMode = _userSettings.DarkModeActive;
        SelectedLanguage = _userSettings.Language;
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
            if (UseDarkMode == _userSettings.DarkModeActive && SelectedLanguage == _userSettings.Language)
                return; // No changes to save

            if (SelectedLanguage != _userSettings.Language)
                MessageBox.Show(Resource.settings_restartApplication);

            _userSettings.Language = SelectedLanguage;
            _userSettings.DarkModeActive = UseDarkMode;

            await _settingsRepository.UpdateSettingsAsync(_userSettings);
            _settingsService.ApplySettings(_userSettings);
        }
        catch (Exception exc)
        {
            _logger.LogExceptionToTempFile(exc);
        }
    }
}
