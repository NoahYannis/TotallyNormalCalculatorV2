using System.Threading.Tasks;

namespace TotallyNormalCalculator.Repository.Settings;

internal interface ISettingsRepository<T> where T : class
{ 
    T GetUserSetting();
    Task UpdateSettingAsync(T settingsModel);
}
