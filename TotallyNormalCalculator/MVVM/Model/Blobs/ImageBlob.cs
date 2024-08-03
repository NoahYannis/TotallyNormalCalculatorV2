using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;

public class ImageBlob : BlobModel
{
    public BitmapImage Image { get; set; }

    public ImageBlob()
    {
        BlobType = BlobType.Image;
    }
}
