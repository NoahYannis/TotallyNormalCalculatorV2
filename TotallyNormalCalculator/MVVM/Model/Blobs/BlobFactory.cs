using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;

public static class BlobFactory
{
    public static BlobType DetermineBlobType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
            return BlobType.Image;

        if (new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".json" }.Contains(extension))
            return BlobType.Video;

        if (new[] { ".mp3", ".wav", ".ogg", ".flac" }.Contains(extension))
            return BlobType.Audio;

        if (new[] { ".txt", ".md", ".csv", ".log", ".pdf" }.Contains(extension))
            return BlobType.Text;

        return BlobType.Other;
    }


    /// <summary>
    /// Creates a concrete blob model based on the blob's file extension.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="blobClient"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<BlobModel> CreateBlobModel(BlobClient blobClient, Stream stream)
    {
        BlobType blobType = DetermineBlobType(blobClient.Name);

        switch (blobType)
        {
            case BlobType.Image:
                return await CreateImageBlob(blobClient, stream);

            case BlobType.Video:
                return await CreateVideoBlob(blobClient, stream);

            case BlobType.Audio:
                return new VideoBlob
                {
                    Name = blobClient.Name,
                  
                };
            case BlobType.Text:
            case BlobType.Other:
                return null;

            default:
                return null;
        }
    }

    private static async Task<BlobModel> CreateVideoBlob(BlobClient blobClient, Stream stream)
    {
        return new VideoBlob
        {
            Name = blobClient.Name,
            VideoUrl = blobClient.Uri.ToString(),
        };
    }


    private static async Task<BlobModel> CreateImageBlob(BlobClient blobClient, Stream stream)
    {
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return new ImageBlob
        {
            Name = blobClient.Name,
            Image = bitmapImage
        };
    }
}

