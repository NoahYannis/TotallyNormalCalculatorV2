using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.ViewModels;
using TotallyNormalCalculator.Repository.Settings;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture]
public class SettingsTests
{
    private Mock<ISettingsRepository<SettingsModel>> _settingsRepository;
    private Mock<ISettingsService> _settingsService;
    private Mock<IMessageBoxService> _messageService;
    private Mock<SettingsViewModel> _settingsViewModel;

    [SetUp]
    public void Setup()
    {
        _settingsRepository = new Mock<ISettingsRepository<SettingsModel>>();
        _settingsService = new Mock<ISettingsService>();
        _messageService = new Mock<IMessageBoxService>();
        _settingsViewModel = new Mock<SettingsViewModel>(_settingsRepository.Object, null, _settingsService.Object, _messageService.Object);

        var userSettings = new SettingsModel
        {
            DarkModeActive = false,
            Language = "en-Us"
        };

        _settingsRepository.Setup(r => r.GetUserSettings()).ReturnsAsync(userSettings);
        _settingsRepository.Setup(r => r.UpdateSettingsAsync(It.IsAny<SettingsModel>())).Returns(Task.CompletedTask);
        _messageService.Setup(Setup => Setup.Show(It.IsAny<string>()));
    }

    [Test]
    public async Task SaveSettings_SameSettings_AreNotSaved()
    {
        await _settingsViewModel.Object.SaveSettings();
        _settingsRepository.Verify(x => x.UpdateSettingsAsync(It.IsAny<SettingsModel>()), Times.Never);
    }

    [Test]
    public async Task SaveSettings_ChangedSettings_ShouldBeApplyed()
    {
        _settingsViewModel.Object.UseDarkMode = false;
        _settingsViewModel.Object.SelectedLanguage = "de-DE";

        await _settingsViewModel.Object.SaveSettings();

        _settingsRepository.Verify(r => r.UpdateSettingsAsync(It.IsAny<SettingsModel>()), Times.Once);
        _messageService.Verify(s => s.Show(It.IsAny<string>()), Times.Once);
        _settingsService.Verify(s => s.ApplySettings(It.IsAny<SettingsModel>()), Times.Once);
    }
}
