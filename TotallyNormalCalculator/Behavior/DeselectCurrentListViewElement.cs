using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TotallyNormalCalculator.Logging;

namespace TotallyNormalCalculator.Behavior;

/// <summary>
/// A behavior to deselect the current item in a ListView when clicking outside of the items.
/// </summary>
public class DeselectCurrentListViewElement : Behavior<UserControl>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var userControl = sender as UserControl;

        var hitTestResult = VisualTreeHelper.HitTest(userControl, e.GetPosition(userControl));

        DependencyObject visualHit = hitTestResult?.VisualHit;

        while (visualHit != null)
        {
            // Don't deselect the entry if its TextBox or a button was clicked
            if (visualHit is TextBox || visualHit is Button)
            {
                return;
            }

            visualHit = VisualTreeHelper.GetParent(visualHit);
        }

        // Find the clicked ListViewItem by traversing up the visual tree
        var clickedListViewItem = FindAncestor<ListViewItem>(hitTestResult.VisualHit);

        // If a ListViewItem was clicked then don't deselect anything
        if (clickedListViewItem != null)
            return;

        dynamic viewModel = userControl.DataContext;

        try
        {
            if (viewModel.SelectedElement != null)
            {
                viewModel.HandleDeselection();
            }
        }
        catch (Exception exc)
        {
            var logger = App.AppHost.Services.GetRequiredService<ITotallyNormalCalculatorLogger>();
            logger.LogExceptionToTempFile(exc);
        }
    }

    /// <summary>
    /// Traverse up the visual tree to find an ancestor of a certain type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="current"></param>
    /// <returns></returns>
    private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T)
            {
                return (T)current;
            }
            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
