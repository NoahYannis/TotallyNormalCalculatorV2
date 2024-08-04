using System.Windows;

namespace TotallyNormalCalculator;
public class MessageBoxService : IMessageBoxService
{
    public MessageBoxResult Show(string message) => MessageBox.Show(message);
    public MessageBoxResult ShowQuestion(string message)
    {
        return MessageBox.Show(message, string.Empty, MessageBoxButton.YesNo);
    }
}
