using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.Core;

/// <summary>
/// A behavior to deselect the current item in a ListView when clicking outside of the items.
/// </summary>
public class DeselectCurrentListViewEntry : Behavior<ListView>
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
        var listView = sender as ListView;
        var hitTestResult = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));

        
        // Check if click was outside of the ListView
        if (hitTestResult == null)
            return;

        // Find the clicked ListViewItem by traversing up the visual tree
        var clickedListViewItem = FindAncestor<ListViewItem>(hitTestResult.VisualHit);

        // If a ListViewItem was clicked then don't deselect anything
        if (clickedListViewItem != null)
            return;

        var viewModel = listView.DataContext as DiaryViewModel;

        if (viewModel?.SelectedEntry != null)
        {
            viewModel.ClearInputFields();
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
