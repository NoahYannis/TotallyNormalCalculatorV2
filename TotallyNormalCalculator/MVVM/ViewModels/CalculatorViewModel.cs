using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
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
    private string _operation;

    [ObservableProperty]
    private double _result;



    private int switchViewCounter;
    private string firstPartOfNumber;
    private string secondPartOfNumber;
    private bool calculationHasError;

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
        if (CalculatorText.Length == 0)
        {
            return;
        }

        try
        {
            CalculatorText = CalculatorText.Remove(CalculatorText.Length - 1, 1);

            if (Operation is null)
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
                calculationHasError = true;
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
        calculationHasError = false;
    }

    [RelayCommand]
    public void AddCharacter(object commandParam)
    {
        string newCharacter = commandParam.ToString();

        if (calculationHasError)
        {
            AllClear();
            return;
        }

        if (CalculatorText.Length == 0 && IsValidFirstCharacter(newCharacter))
        {
            CalculatorText = newCharacter;

            if (newCharacter == "√")
            {
                Operation = CalculatorText.Length == 1 ? "√" : null;
            }
        }
        else if (CalculatorText.Length > 0 && newCharacter == "." && !CalculatorText.Contains('.'))
        {
            CalculatorText += ".";
        }
        else
        {
            CalculatorText += newCharacter;
        }

        ProcessNewCharacter(newCharacter);
    }




    #endregion

    partial void OnResultChanging(double oldValue, double newValue)
    {
        CalculatorText = newValue.ToString();
    }

    private void ProcessNewCharacter(string newCharacter)
    {
        if (CalculatorText.Length > 1)
        {
            SetOperation(newCharacter);
        }

        try
        {
            if (Operation == null)
            {
                FirstNumber = SetCalculationNumber(newCharacter, numberToSet: FirstNumber);
            }
            else
            {
                SecondNumber = SetCalculationNumber(newCharacter, numberToSet: SecondNumber);
            }
        }
        catch (Exception)
        {
            CalculatorText = "An error occurred";
        }
    }

    private bool IsValidFirstCharacter(string newCharacter)
    {
        return double.TryParse(newCharacter, out double _) || newCharacter == "-" || newCharacter == "√";
    }

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



    private double SetCalculationNumber(string newCharacter, double numberToSet)
    {
        double number = 0;

        try
        {
            if (CalculatorText.Length == 0)
            {
                return Convert.ToDouble(newCharacter);
            }

            if (!CalculatorText.Contains('.'))
            {
                number = (numberToSet * 10) + Convert.ToDouble(newCharacter);
                return number;
            }

            int decimalIndex = CalculatorText.IndexOf(".");
            firstPartOfNumber = CalculatorText.Substring(0, decimalIndex);
            secondPartOfNumber = CalculatorText.Substring(decimalIndex, CalculatorText.Length - decimalIndex);
            return Convert.ToDouble(firstPartOfNumber + secondPartOfNumber);
        }

        catch (Exception)
        {
        }

        return number;
    }
}