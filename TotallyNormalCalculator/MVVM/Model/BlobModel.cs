using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model;

public class BlobModel : IModel
{
    public string Name { get; set; }
    public BitmapImage Image { get; set; }
}
