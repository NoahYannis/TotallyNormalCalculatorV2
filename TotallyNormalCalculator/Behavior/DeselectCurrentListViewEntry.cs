using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.Behavior;

/// <summary>
/// A behavior to deselect the current item in a ListView when clicking outside of the items.
/// </summary>
public class DeselectCurrentListViewEntry : Behavior<UserControl>
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

        var viewModel = userControl.DataContext as DiaryViewModel;

        if (viewModel?.SelectedEntry != null)
        {
            viewModel.ClearInputFields();
            viewModel.SelectedEntry = null;
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
