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
    private string firstPart;
    private string secondPart;


    #region Commands

    [RelayCommand]
    public void MaximizeWindow()
    {
        if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
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
    public void RemoveCharacters()
    {
        try
        {
            CalculatorText = CalculatorText.Remove(CalculatorText.Length - 1, 1);

            if (Operation is null) // It's still the first Number
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

            case null:
                CalculatorText = "Invalid operation";
                break;
        }

        try
        {
            FirstNumber = Convert.ToDouble(Result);
        }
        catch (Exception)
        {
            FirstNumber = 0;
            Result = 0;
        }

        SecondNumber = 0;
        Operation = null;
        switchViewCounter = 0;
    }

    [RelayCommand]
    public void AllClear()
    {
        FirstNumber = 0;
        SecondNumber = 0;
        Operation = null;
        Result = 0;
        CalculatorText = "";
        switchViewCounter = 0;
    }

    [RelayCommand]
    public void AddCharacters(object commandParam)
    {
        string character = commandParam.ToString();
        string charTest = character;
        bool IsValidInput = charTest.All(x => char.IsDigit(x) || '-' == x || '√' == x);

        if (CalculatorText is "Invalid operation")
        {
            FirstNumber = 0;
            SecondNumber = 0;
            Operation = null;
            Result = 0;
            CalculatorText = "";
            switchViewCounter = 0;
        }

        if (CalculatorText.Length is 0)
        {
            if (IsValidInput)
            {
                CalculatorText = character;
            }
        }
        else if (character is ".")
        {
            if (CalculatorText.Contains(".") is false)
            {
                CalculatorText += ".";
            }
        }
        else
        {
            if (character is not "√")
            {
                CalculatorText += character;
            }
            else
            {
                Operation = null;
            }
        }


        if (CalculatorText.Length > 1)
        {
            switch (character)
            {
                case "+":
                    Operation = "+";
                    CalculatorText = "";
                    break;

                case "-":
                    Operation = "-";
                    CalculatorText = "";
                    break;

                case "×":
                    Operation = "×";
                    CalculatorText = "";
                    break;

                case "÷":
                    Operation = "÷";
                    CalculatorText = "";
                    break;

                case "^":
                    Operation = "^";
                    CalculatorText = "";
                    break;

                default:
                    break;
            }
        }
        else if (CalculatorText.Length == 1 && character is "√")
        {
            Operation = "√";
        }

        if (Operation is null)
        {
            if (CalculatorText.Length is 0)
            {
                try
                {
                    FirstNumber = Convert.ToInt64(character);
                }
                catch (Exception)
                {
                    FirstNumber = 0;
                }
            }
            else
            {
                try
                {
                    if (character is not "." && CalculatorText.Contains(".") is false && CalculatorText.StartsWith("-") is false)  // whole, not negative number
                    {
                        FirstNumber = (FirstNumber * 10) + Convert.ToInt64(character);
                    }
                    else if (CalculatorText.StartsWith("-"))
                    {
                        FirstNumber = Convert.ToDouble(CalculatorText.Substring(0, CalculatorText.Length));

                        if (CalculatorText.Contains("."))
                        {
                            firstPart = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                            secondPart = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                            FirstNumber = Convert.ToDouble(firstPart + secondPart);
                        }
                    }
                    else // number is a decimal and is not negative
                    {
                        firstPart = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                        secondPart = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                        FirstNumber = Convert.ToDouble(firstPart + secondPart);
                    }
                }
                catch (Exception)
                {
                    FirstNumber = 0;
                }
            }
        }
        else // operation not null -> second number
        {
            if (CalculatorText.Length is 0)
            {
                try
                {
                    SecondNumber = Convert.ToInt64(character);
                }
                catch (Exception)
                {
                    SecondNumber = 0;
                }
            }
            else
            {
                try
                {
                    if (character is not "." && CalculatorText.Contains(".") is false && CalculatorText.StartsWith("-") is false)
                    {
                        SecondNumber = (SecondNumber * 10) + Convert.ToInt64(character);
                    }
                    else if (CalculatorText.StartsWith("-"))
                    {
                        SecondNumber = Convert.ToDouble(CalculatorText.Substring(0, CalculatorText.Length));

                        if (CalculatorText.Contains("."))
                        {
                            firstPart = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                            secondPart = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                            SecondNumber = Convert.ToDouble(firstPart + secondPart);
                        }
                    }
                    else
                    {
                        firstPart = CalculatorText.Substring(0, CalculatorText.IndexOf("."));
                        secondPart = CalculatorText.Substring(CalculatorText.IndexOf("."), CalculatorText.Length - CalculatorText.IndexOf("."));

                        SecondNumber = Convert.ToDouble(firstPart + secondPart);
                    }
                }
                catch (Exception)
                {
                    SecondNumber = 0;
                }
            }
        }
    }
}

#endregion