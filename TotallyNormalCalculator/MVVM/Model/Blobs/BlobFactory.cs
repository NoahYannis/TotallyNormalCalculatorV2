﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TotallyNormalCalculator.MVVM.Model.Blobs;

public class BlobFactory : IBlobFactory
{
    private static BlobType DetermineBlobType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
            return BlobType.Image;

        if (new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".json" }.Contains(extension))
            return BlobType.Video;

        return BlobType.Other;
    }

    public string GetBlobName(string path) => Path.GetFileName(path);

    /// <summary>
    /// Creates a concrete blob model based on the blob's file extension.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="blobName"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<BlobModel> CreateBlobModel(string blobName, string contentBase64)
    {
        BlobType blobType = DetermineBlobType(blobName);

        return blobType switch
        {
            BlobType.Image => CreateImageBlob(blobName, contentBase64),
            BlobType.Video => await CreateVideoBlob(blobName, contentBase64),
            BlobType.Other => null,
            _ => null
        };
    }

    public bool IsAllowedBlobType(string fileName) => DetermineBlobType(fileName) != BlobType.Other;



    private static async Task<BlobModel> CreateVideoBlob(string blobName, string contentBase64)
    {
        string localFilePath = Path.Combine(Path.GetTempPath(), blobName);
        {
            byte[] bytes = Convert.FromBase64String(contentBase64);

            if (FileIsEmpty(localFilePath))
            {
                await File.WriteAllBytesAsync(localFilePath, bytes);
            }

            return new VideoBlob
            {
                Name = blobName,
                VideoUrl = localFilePath
            };
        }

    }


    /// <summary>
    /// Creates the blob bitmap from its base64 content.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="contentBase64"></param>
    /// <returns></returns>
    private static ImageBlob CreateImageBlob(string imageName, string contentBase64)
    {
        byte[] bytes = Convert.FromBase64String(contentBase64);

        using var stream = new MemoryStream(bytes);

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

    private static bool FileIsEmpty(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        return new FileInfo(filePath).Length == 0;
    }
}

