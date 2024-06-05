using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using TotallyNormalCalculator.Core;

namespace TotallyNormalCalculator;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); ;
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); ;

        FrameworkElement.LanguageProperty.OverrideMetadata(
          typeof(FrameworkElement),
          new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        DBHelper.EnsureDatabaseExists();

        base.OnStartup(e);
    }
}
