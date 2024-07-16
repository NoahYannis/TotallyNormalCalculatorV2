using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.Settings;

internal interface ISettingsRepository<T> where T : class
{
    Task<T> GetUserSettings();
    Task UpdateSettingsAsync(T settingsModel);
}
