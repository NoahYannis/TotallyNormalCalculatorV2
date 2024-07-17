using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;

public static class BlobFactory
{
    private static BlobType DetermineBlobType(string fileName)
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
    /// <param name="blobName"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<BlobModel> CreateBlobModel(string blobName, Stream stream)
    {
        BlobType blobType = DetermineBlobType(blobName);

        return blobType switch
        {
            BlobType.Image => CreateImageBlob(blobName, stream),
            BlobType.Video => await CreateVideoBlob(blobName, stream),
            BlobType.Audio => new VideoBlob
            {
                Name = blobName,


            },
            BlobType.Text or BlobType.Other => null,
            _ => null,
        };
    }

    private static async Task<BlobModel> CreateVideoBlob(string blobName, Stream stream)
    {
        // To do: Find a way to do this without saving the file to disk
        string localFilePath = Path.Combine(Path.GetTempPath(), blobName);

        using (FileStream fileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
        {
            await stream.CopyToAsync(fileStream);
        }

        return new VideoBlob
        {
            Name = blobName,
            VideoUrl = localFilePath
        };
    }


    private static BlobModel CreateImageBlob(string imageName, Stream stream)
    {
        stream.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return new ImageBlob
        {
            Name = imageName,
            Image = bitmapImage
        };
    }
}

