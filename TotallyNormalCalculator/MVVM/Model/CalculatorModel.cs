using CommunityToolkit.Mvvm.ComponentModel;

namespace TotallyNormalCalculator.MVVM.Model;

public class CalculatorModel : ObservableObject
{
    public static double Add(double x, double y) => x + y;

    public static double Subtract(double x, double y) => x - y;

    public static double Multiply(double x, double y) => x * y;

    public static double Divide(double x, double y) => y is 0 ? 0 : x / y;
}
