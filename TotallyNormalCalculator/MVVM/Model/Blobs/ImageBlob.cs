using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;

class ImageBlob : BlobModel
{
    public BitmapImage Image { get; set; }

    public ImageBlob()
    {
        BlobType = BlobType.Image;
    }
}
