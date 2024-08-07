using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using TotallyNormalCalculator.MVVM.ViewModels;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace TotallyNormalCalculator.UnitTests.ViewModels;

[TestFixture, Apartment(ApartmentState.STA)]
public class WebViewTests
{
    private Mock<IMessageBoxService> _messageBox;
    private WebViewViewModel _webViewVM;
    private Mock<WebView2> _webView;
    private Mock<CoreWebView2> _mockCoreWebView2;


    [SetUp]
    public void Setup()
    {
        _messageBox = new Mock<IMessageBoxService>();
        _webViewVM = new WebViewViewModel(_messageBox.Object);
        _webView = new Mock<WebView2>();
        _mockCoreWebView2 = new Mock<CoreWebView2>();
        _webView.Setup(w => w.CoreWebView2).Returns(_mockCoreWebView2.Object);
        _webView.Setup(w => w.EnsureCoreWebView2Async(It.IsAny<CoreWebView2Environment>(), It.IsAny<CoreWebView2ControllerOptions>())).Returns(Task.CompletedTask);
        _mockCoreWebView2.Setup(cw => cw.Navigate(It.IsAny<string>())).Verifiable();
    }

    [Test]
    [TestCase("")]
    [TestCase("youtube")]
    [Ignore("TODO: Find out how to mock WebView correctly with readonly CoreWebView2 property")]
    public void Navigate_WithInvalidUrl_ThrowsErrorAndDisplaysMessage(string url)
    {

        // Arrange
        _webViewVM.Url = url;

        // Act
        _webViewVM.NavigateToUrl(_webView.Object);

        // Assert
        Assert.Throws<ArgumentException>(() => _webViewVM.NavigateToUrl(_webView.Object));
        _messageBox.Verify(m => m.Show(It.IsAny<string>()), Times.Once);
    }


    [Test]
    [Ignore("TODO: Find out how to mock WebView correctly with readonly CoreWebView2 property")]
    public void Navigate_WithValidUrl_DoesNotThrowOrDisplayMessage()
    {
        // Arrange
        _webViewVM.Url = "https://www.youtube.com";

        // Act
        _webViewVM.NavigateToUrl(_webView.Object);

        // Assert
        Assert.DoesNotThrow(() => _webViewVM.NavigateToUrl(_webView.Object));
        _messageBox.Verify(m => m.Show(It.IsAny<string>()), Times.Never);
    }
}
