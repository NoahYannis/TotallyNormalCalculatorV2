using NUnit.Framework;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture]
public class CalculatorTests
{
    private CalculatorViewModel _calculatorViewModel;

    [SetUp]
    public void SetUp()
    {
        _calculatorViewModel = new CalculatorViewModel(logger: null) { SelectedViewModel = new CalculatorViewModel(null)};
    }


    [Test]
    public void RemoveCharacter_WithNoText_DoesntThrow()
    {
        Assert.DoesNotThrow(_calculatorViewModel.RemoveCharacter);
    }


    [Test]
    public void RemoveCharacter_WithOneChar_RemovesCharAndSetsFirstNumberToZero()
    {
        _calculatorViewModel.CalculatorText = "1";

        Assert.DoesNotThrow(_calculatorViewModel.RemoveCharacter);
        Assert.That(_calculatorViewModel.CalculatorText, Is.Empty);
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(0));
    }


    [Test]
    public void RemoveCharacter_WithTwoChars_RemovesLastCharAndSetsFirstNumberCorrectly()
    {
        _calculatorViewModel.CalculatorText = "12";

        Assert.DoesNotThrow(_calculatorViewModel.RemoveCharacter);
        Assert.That(_calculatorViewModel.CalculatorText, Is.EqualTo("1"));
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(1));
    }


    [Test]
    public void RemoveCharacter_WithOperationSet_SetsSecondNumberCorrectly()
    {
        _calculatorViewModel.FirstNumber = 12;
        _calculatorViewModel.Operation = "+";
        _calculatorViewModel.CalculatorText = "24";

        Assert.DoesNotThrow(_calculatorViewModel.RemoveCharacter);
        Assert.That(_calculatorViewModel.CalculatorText, Is.EqualTo("2"));
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(12));
        Assert.That(_calculatorViewModel.SecondNumber, Is.EqualTo(2));
    }


    [Test]
    public void SetOperation_EmptiesCalculatorText()
    {
        _calculatorViewModel.SetOperation("+");
        Assert.That(_calculatorViewModel.CalculatorText, Is.Empty);       
    }


    [Test]
    public void SetOperation_SquareRoot_DoesNotEmptyCalculatorText()
    {
        _calculatorViewModel.CalculatorText = "12";
        _calculatorViewModel.SetOperation("√");
        Assert.That(_calculatorViewModel.CalculatorText, Is.Not.Empty);
    }



    [Test]
    public void Calculate_AfterCalculation_FirstNumberIsSetToResult()
    {
        _calculatorViewModel.FirstNumber = 12;
        _calculatorViewModel.SecondNumber = 24;
        _calculatorViewModel.Operation = "+";

        _calculatorViewModel.Calculate();

        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(36));
        Assert.That(_calculatorViewModel.SecondNumber, Is.EqualTo(0));
    }



    [Test]
    public void AddCharacter_FirstCharacterIsNumber_GetsSetCorrectly()
    {
        _calculatorViewModel.AddCharacter("2");
        Assert.That(_calculatorViewModel.CalculatorText, Is.EqualTo("2"));
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(2));
    }


    [Test]
    public void AddCharacter_DotAsFirstChar_DoesNotGetAdded()
    {
        _calculatorViewModel.AddCharacter(".");
        Assert.That(_calculatorViewModel.CalculatorText, Is.Empty);
    }


    [Test]
    public void AddCharacter_DotAsSecondChar_DoesGetAdded()
    {
        _calculatorViewModel.CalculatorText = "2";
        _calculatorViewModel.AddCharacter(".");
        Assert.That(_calculatorViewModel.CalculatorText, Is.Not.Empty);
    }



    [Test]
    public void AddCharacter_AlreadyContainsDot_DoesNotGetAddedAgain()
    {
        _calculatorViewModel.AddCharacter(2);
        _calculatorViewModel.AddCharacter(".");
        _calculatorViewModel.AddCharacter(".");
        Assert.That(_calculatorViewModel.CalculatorText, Is.EqualTo("2."));
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(2));
    }



    [Test]
    public void UpdateCalculationNumber_WithNoOperation_SetsFirstNumberCorrectly()
    {
        _calculatorViewModel.CalculatorText = "12";
        _calculatorViewModel.UpdateCalculationNumber();
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(12));
    }



    [Test]
    public void UpdateCalculationNumber_WithOperation_SetsSecondNumberCorrectly()
    {
        _calculatorViewModel.FirstNumber = 12;
        _calculatorViewModel.Operation = "+";
        _calculatorViewModel.CalculatorText = "24";
        _calculatorViewModel.UpdateCalculationNumber();
        Assert.That(_calculatorViewModel.FirstNumber, Is.EqualTo(12));
        Assert.That(_calculatorViewModel.SecondNumber, Is.EqualTo(24));
    }




}
