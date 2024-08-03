namespace TotallyNormalCalculator.MVVM.Model.Blobs;

public class VideoBlob : BlobModel
{
    public string VideoUrl { get; set; }

    public VideoBlob()
    {
        BlobType = BlobType.Video;
    }
}
