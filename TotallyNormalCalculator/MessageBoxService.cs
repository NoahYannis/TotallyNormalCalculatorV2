using System.Windows;

namespace TotallyNormalCalculator;
public class MessageBoxService : IMessageBoxService
{
    public void Show(string message)
    {
        MessageBox.Show(message);
    }
}
