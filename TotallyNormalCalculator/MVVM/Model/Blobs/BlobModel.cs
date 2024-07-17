using System.IO;

namespace TotallyNormalCalculator.MVVM.Model;

public class BlobModel : IModel
{
    public string Name { get; set; }
    public Stream Content { get; set; }
    public BlobType BlobType { get; set; }
}

public enum BlobType
{
    Image,
    Video,
    Audio,
    Text,
    Other
}
