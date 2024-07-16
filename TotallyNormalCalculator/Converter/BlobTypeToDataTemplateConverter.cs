using System.Windows;
using System.Windows.Controls;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Converter;

public class BlobTypeToDataTemplateConverter : DataTemplateSelector
{
    public DataTemplate ImageBlobTemplate { get; set; }
    public DataTemplate VideoBlobTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var blob = item as BlobModel;

        switch (blob?.BlobType)
        {
            case BlobType.Image:
                return ImageBlobTemplate;
            case BlobType.Video:
                return VideoBlobTemplate;


            default:
                return ImageBlobTemplate;
        }
    }
}
