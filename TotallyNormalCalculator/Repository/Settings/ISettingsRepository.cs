using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.Settings;

internal interface ISettingsRepository<T> where T : class
{ 
    T GetUserSettings();
    Task UpdateSettingsAsync(T settingsModel);
}
