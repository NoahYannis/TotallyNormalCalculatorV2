namespace TotallyNormalCalculator.MVVM.Model.Blobs;

class TextBlob : BlobModel
{
    public string Text { get; set; }

    public TextBlob()
    {
        BlobType = BlobType.Text;
    }
}
