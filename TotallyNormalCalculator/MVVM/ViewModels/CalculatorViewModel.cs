using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
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

    private int _switchViewCounter;

    #region Commands


    [RelayCommand]
    public void SwitchView()
    {
        _switchViewCounter++;

        if (_switchViewCounter == 4)
        {
            SelectedViewModel = new DiaryViewModel(logger, new DiaryRepositoryDapper(logger));
            _switchViewCounter = 0;
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

        if (calcOperation != "√")
        {
            CalculatorText = string.Empty;
        }
    }

    [RelayCommand]
    public void Calculate()
    {
        Result = Operation switch
        {
            "+" => CalculatorModel.Add(FirstNumber, SecondNumber),
            "-" => CalculatorModel.Subtract(FirstNumber, SecondNumber),
            "×" => CalculatorModel.Multiply(FirstNumber, SecondNumber),
            "÷" => CalculatorModel.Divide(FirstNumber, SecondNumber),
            "^" => Math.Pow(FirstNumber, SecondNumber),
            "√" => Math.Sqrt(SecondNumber),
            _ => 0
        };

        try
        {
            FirstNumber = Result; // Continue calculation with the result as the first number
        }
        catch (Exception exc)
        {
            AllClear();
            logger.LogExceptionToTempFile(exc);
        }

        SecondNumber = 0;
        _switchViewCounter = 0;
    }

    [RelayCommand]
    public void AllClear()
    {
        FirstNumber = SecondNumber = Result = _switchViewCounter = 0;
        Operation = null;
        CalculatorText = string.Empty;
    }

    [RelayCommand]
    public void AddCharacter(object commandParam)
    {
        string newCharacter = commandParam.ToString();

        if (CalculatorText.Length == 0)
        {
            if (!IsValidFirstCharacter(newCharacter))
            {
                return;
            }

            CalculatorText = newCharacter;
            UpdateCalculationNumber();
            return;
        }

        if (newCharacter == "." && CalculatorText.Contains('.'))
        {
            return; // Only one decimal point allowed
        }

        CalculatorText += newCharacter;
        UpdateCalculationNumber();
    }



    #endregion

    partial void OnResultChanging(double oldValue, double newValue)
    {
        CalculatorText = newValue.ToString();
    }

    private void UpdateCalculationNumber()
    {
        try
        {
            if (Operation == null)
            {
                FirstNumber = Convert.ToDouble(CalculatorText, CultureInfo.InvariantCulture);
            }
            else
            {
                SecondNumber = Convert.ToDouble(CalculatorText, CultureInfo.InvariantCulture);
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
}