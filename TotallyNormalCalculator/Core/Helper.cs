using System.Configuration;

namespace TotallyNormalCalculator.Core;

public static class Helper
{
    public static string GetConnectionString(string name)
    {
        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
