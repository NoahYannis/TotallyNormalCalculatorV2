using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using System;
using TotallyNormalCalculator.Languages;

namespace TotallyNormalCalculator.MVVM.ViewModels;
public partial class WebViewViewModel(IMessageBoxService messageBox) : BaseViewModel
{

    [ObservableProperty]
    public string _url;

    [RelayCommand]
    private void NavigateToUrl(WebView2 webView)
    {
        try
        {
            webView.CoreWebView2.Navigate(Url);
        }
        catch (ArgumentException)
        {
            // Try the URL again with https://
            try
            {
                webView.CoreWebView2.Navigate($"https://{Url}");
            }
            catch (Exception)
            {
                messageBox.Show(Resource.webView_enterValidUrl);
            }
        }
    }
}
