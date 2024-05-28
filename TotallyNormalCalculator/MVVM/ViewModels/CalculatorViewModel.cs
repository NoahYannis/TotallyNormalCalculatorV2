using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class CalculatorViewModel : ObservableObject
{

    [ObservableProperty]
    private ObservableObject _selectedViewModel;

    [ObservableProperty]
    private string _calculatorText = string.Empty;

    [ObservableProperty]
    private double _firstNumber;

    [ObservableProperty]
    private double _secondNumber;

    [ObservableProperty]
    private string _operation = string.Empty;

    [ObservableProperty]
    private double _result;

    partial void OnResultChanging(double oldValue, double newValue)
    {
        CalculatorText = newValue.ToString();
    }


    private int switchViewCounter;
    private string firstPartOfNumber;
    private string secondPartOfNumber;


    #region Commands

    [RelayCommand]
    public void MaximizeWindow()
    {
        Application.Current.MainWindow.WindowState =
            Application.Current.MainWindow.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
    }


    [RelayCommand]
    public void MinimizeWindow()
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }


    [RelayCommand]
    public void CloseWindow()
    {
        Application.Current.Shutdown();
    }


    [RelayCommand]
    public void SwitchView()
    {
        switchViewCounter++;

        if (switchViewCounter == 4)
        {
            SelectedViewModel = new DiaryViewModel();
            switchViewCounter = 0;
        }
    }

    [RelayCommand]
    public void RemoveCharacter()
    {
        if (string.IsNullOrEmpty(CalculatorText))
        {
            return;
        }

        try
        {
            CalculatorText = CalculatorText.Remove(CalculatorText.Length - 1, 1);

            if (Operation is null) // First number hasn't been entered yet
            {
                FirstNumber = Convert.ToDouble(CalculatorText);
            }
            else
            {
                SecondNumber = Convert.ToDouble(CalculatorText);
            }
        }
        catch (Exception)
        {

        }
    }

    [RelayCommand]
    public void Calculate()
    {
        switch (Operation)
        {
            case "+":
                Result = CalculatorModel.Add(FirstNumber, SecondNumber);
                break;

            case "-":
                Result = CalculatorModel.Subtract(FirstNumber, SecondNumber);
                break;

            case "×":
                Result = CalculatorModel.Multiply(FirstNumber, SecondNumber);
                break;

            case "÷":
                Result = CalculatorModel.Divide(FirstNumber, SecondNumber);
                break;

            case "^":
                Result = Math.Pow(FirstNumber, SecondNumber);
                break;

            case "√":
                Result = Math.Sqrt(SecondNumber);
                break;

            default:
                AllClear();
                CalculatorText = "Invalid operation";
                return;
        }

        try
        {
            FirstNumber = Convert.ToDouble(Result); // Continue calculation with the result as the first number
        }
        catch (Exception)
        {
            AllClear();
        }

        SecondNumber = 0;
        Operation = null;
        switchViewCounter = 0;
    }

    [RelayCommand]
    public void AllClear()
    {
        FirstNumber = SecondNumber = Result = switchViewCounter = 0;
        Operation = null;
        CalculatorText = string.Empty;
    }

    [RelayCommand]
    public void AddCharacter(object commandParam)
    {
        string newCharacter = commandParam.ToString();
        bool IsValidInput = newCharacter.All(x => char.IsDigit(x) || '-' == x || '√' == x);

        if (CalculatorText is "Invalid operation")
        {
            AllClear();
        }

        if (IsValidInput && !string.IsNullOrEmpty(CalculatorText))
        {
            CalculatorText = newCharacter;
        }
        else if (newCharacter is "." && !CalculatorText.Contains('.'))
        {
            CalculatorText += ".";
        }
        else
        {
            if (newCharacter is not "√")
            {
                CalculatorText += newCharacter;
            }
            else
            {
                Operation = null;
            }
        }


        if (CalculatorText.Length > 1)
        {
            SetOperation(newCharacter);
        }
        else if (CalculatorText.Length == 1 && newCharacter is "√")
        {
            Operation = "√";
        }

        if (Operation is null)
        {
            if (CalculatorText.Length is 0)
            {
                try
                {
                    FirstNumber = Convert.ToInt64(newCharacter);
                }
                catch (Exception)
                {
                    FirstNumber = 0;
                }
            }
            else
            {
                FirstNumber = SetCalculationNumber(newCharacter);
            }
        }
        else // operation not null -> second number
        {
            if (string.IsNullOrEmpty(CalculatorText))
            {
                try
                {
                    SecondNumber = Convert.ToInt64(newCharacter);
                }
                catch (Exception)
                {
                    SecondNumber = 0;
                }
            }
            else
            {
                SecondNumber = SetCalculationNumber(newCharacter);
            }
        }
    }


    #endregion


    private void SetOperation(string newCharacter)
    {
        switch (newCharacter)
        {
            case "+":
            case "-":
            case "×":
            case "÷":
            case "^":
                Operation = newCharacter;
                CalculatorText = string.Empty;
                break;

            default:
                break;
        }
    }


    private double SetCalculationNumber(string newCharacter)
    {
        double number = 0;

        try
        {
            if (newCharacter is not "." && !CalculatorText.Contains('.') && !CalculatorText.StartsWith("-"))  // positive integer
            {
                number = (number * 10) + Convert.ToInt64(newCharacter);
            }
            else if (CalculatorText.StartsWith("-"))
            {
                number = Convert.ToDouble(CalculatorText.Substring(0, CalculatorText.Length));

                if (CalculatorText.Contains("."))
                {
                    firstPartOfNumber = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                    secondPartOfNumber = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                    number = Convert.ToDouble(firstPartOfNumber + secondPartOfNumber);
                }
            }
            else // number is a decimal and is not negative
            {
                firstPartOfNumber = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                secondPartOfNumber = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                number = Convert.ToDouble(firstPartOfNumber + secondPartOfNumber);
            }
        }
        catch (Exception)
        {
            number = 0;
        }

        return number;
    }
}