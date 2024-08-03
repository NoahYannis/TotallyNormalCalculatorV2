using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TotallyNormalCalculator.Languages;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsRepository<SettingsModel> _settingsRepository;
    private readonly ITotallyNormalCalculatorLogger _logger;
    private readonly ISettingsService _settingsService;
    private readonly IMessageBoxService _messageService;
    private readonly SettingsModel _userSettings;

    public SettingsViewModel
        (ISettingsRepository<SettingsModel> settingsRepository,
        ITotallyNormalCalculatorLogger logger,
        ISettingsService settingsService,
        IMessageBoxService messageService)
    {
        (_settingsRepository, _logger, _settingsService, _messageService) = (settingsRepository, logger, settingsService, messageService);
        _userSettings = Task.Run(() => _settingsRepository.GetUserSettings()).GetAwaiter().GetResult();
        UseDarkMode = _userSettings.DarkModeActive;
        SelectedLanguage = _userSettings.Language;
    }


    [ObservableProperty]
    public string _selectedLanguage;

    [ObservableProperty]
    public bool _useDarkMode;


    [RelayCommand]
    public async Task SaveSettings()
    {
        try
        {
            if (UseDarkMode == _userSettings.DarkModeActive && SelectedLanguage == _userSettings.Language)
                return; // No changes to save

            if (SelectedLanguage != _userSettings.Language)
                _messageService.Show(Resource.settings_restartApplication);

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
