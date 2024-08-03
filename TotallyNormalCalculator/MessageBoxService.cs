using System.Windows;

namespace TotallyNormalCalculator;
public class MessageBoxService : IMessageBoxService
{
    public MessageBoxResult Show(string message)
    {
        return MessageBox.Show(message);
    }
}
