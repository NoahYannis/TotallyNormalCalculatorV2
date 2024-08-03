using System.Windows;
using TotallyNormalCalculator.MVVM.ViewModels;
using Xunit;

namespace TotallyNormalCalculator.UnitTests.CalculatorTests;
internal class Commands
{

    public class CalculatorViewModelTests
    {

        public CalculatorViewModel viewModel = new CalculatorViewModel(logger: null);


        [Fact]
        public void RemoveCharacterCommand_ShouldRemoveCharacter()
        {
            // Arrange
            viewModel.CalculatorText = "123";

            // Act
            viewModel.RemoveCharacterCommand.Execute(null);

            // Assert
            Assert.Equal("12", viewModel.CalculatorText);
        }

        [Fact]
        public void SetOperationCommand_ShouldSetOperation()
        {
            // Act
            viewModel.SetOperationCommand.Execute("+");

            // Assert
            Assert.Equal("+", viewModel.Operation);
            Assert.Equal(string.Empty, viewModel.CalculatorText);
        }

        [Fact]
        public void CalculateCommand_ShouldPerformCalculation()
        {
            // Arrange
            viewModel.FirstNumber = 5;
            viewModel.SecondNumber = 3;
            viewModel.Operation = "+";

            // Act
            viewModel.CalculateCommand.Execute(null);

            // Assert
            Assert.Equal(8, viewModel.Result);
            Assert.Equal(8, viewModel.FirstNumber);
            Assert.Equal(0, viewModel.SecondNumber);
            Assert.Null(viewModel.Operation);

            viewModel.FirstNumber = 2.5;
            viewModel.SecondNumber = 2;
            viewModel.SetOperation("x");
            viewModel.CalculateCommand.Execute(null);

            Assert.Equal(5, viewModel.Result);
            Assert.Equal(5, viewModel.FirstNumber);
        }

        [Fact]
        public void AllClearCommand_ShouldClearCalculator()
        {
            // Arrange
            viewModel.FirstNumber = 5;
            viewModel.SecondNumber = 3;
            viewModel.Result = 8;
            viewModel.Operation = "+";

            // Act
            viewModel.AllClearCommand.Execute(null);

            // Assert
            Assert.Equal(0, viewModel.FirstNumber);
            Assert.Equal(0, viewModel.SecondNumber);
            Assert.Equal(0, viewModel.Result);
            Assert.Null(viewModel.Operation);
            Assert.Equal(string.Empty, viewModel.CalculatorText);
        }

        [Fact]
        public void AddCharacterCommand_ShouldAddCharacter()
        {
            // Act
            viewModel.AddCharacterCommand.Execute("1");

            // Assert
            Assert.Equal("1", viewModel.CalculatorText);
        }
    }
}

