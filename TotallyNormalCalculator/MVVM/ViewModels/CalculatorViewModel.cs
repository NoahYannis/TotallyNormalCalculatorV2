using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.Repository;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class CalculatorViewModel(ITotallyNormalCalculatorLogger logger) : BaseViewModel
{
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
    public void SwitchView()
    {
        switchViewCounter++;

        if (switchViewCounter == 4)
        {
            SelectedViewModel = new DiaryViewModel(logger, new DiaryRepositoryDapper(logger));
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

            if (Operation == null)
            {
                FirstNumber = double.Parse(CalculatorText);
            }
            else
            {
                SecondNumber = double.Parse(CalculatorText);
            }
        }
        catch (Exception exc)
        {
            logger.LogExceptionToTempFile(exc);
        }
    }

    [RelayCommand]
    public void SetOperation(string calcOperation)
    {
        Operation = calcOperation;

        if (calcOperation == "√")
        {
            CalculatorText = "√";
            return;
        }

        CalculatorText = string.Empty;
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
        catch (Exception exc)
        {
            AllClear();
            logger.LogExceptionToTempFile(exc);
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
        }
        else if (CalculatorText.Length > 0 && newCharacter == "." && !CalculatorText.Contains('.'))
        {
            CalculatorText += ".";
        }
        else
        {
            CalculatorText += newCharacter;
        }

        UpdateCalculationNumber(newCharacter);
    }


    #endregion

    partial void OnResultChanging(double oldValue, double newValue)
    {
        CalculatorText = newValue.ToString();
    }

    private void UpdateCalculationNumber(string newNumber)
    {
        try
        {
            if (Operation == null)
            {
                FirstNumber = SetCalculationNumber(newNumber, numberToSet: FirstNumber);
            }
            else
            {
                SecondNumber = SetCalculationNumber(newNumber, numberToSet: SecondNumber);
            }
        }
        catch (Exception exc)
        {
            logger.LogExceptionToTempFile(exc);
        }
    }

    private bool IsValidFirstCharacter(string newCharacter)
    {
        return double.TryParse(newCharacter, out double _) || newCharacter == "-";
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
                return (numberToSet * 10) + Convert.ToDouble(newCharacter);
            }

            int decimalIndex = CalculatorText.IndexOf(".");
            firstPartOfNumber = CalculatorText.Substring(0, decimalIndex);
            secondPartOfNumber = CalculatorText.Substring(decimalIndex, CalculatorText.Length - decimalIndex);
            return Convert.ToDouble(firstPartOfNumber + secondPartOfNumber);
        }
        catch (Exception exc)
        {
            logger.LogExceptionToTempFile(exc);
        }

        return number;
    }
}