using System.Windows;

namespace TotallyNormalCalculator;

public interface IMessageBoxService
{
    MessageBoxResult Show(string message);
    MessageBoxResult ShowQuestion(string message);

}