using System.Windows;

namespace TotallyNormalCalculator;
public class MessageBoxService : IMessageBoxService
{
    public MessageBoxResult Show(string message) => MessageBox.Show(message);
}
