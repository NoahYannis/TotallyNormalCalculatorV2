﻿using System.Windows.Controls;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.MVVM.Views;

public partial class CalculatorView : UserControl
{
    public CalculatorView()
    {
        InitializeComponent();
        this.DataContext = App.AppHost.Services.GetService(typeof(CalculatorViewModel));
    }
}
