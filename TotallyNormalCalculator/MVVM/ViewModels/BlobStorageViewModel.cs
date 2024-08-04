using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.MVVM.Model.Blobs;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.MVVM.ViewModels;

public partial class BlobStorageViewModel
    (ITotallyNormalCalculatorLogger _blobLogger,
    IBlobStorageRepository<BlobModel> _blobStorageRepository,
    IMessageBoxService _messageService,
    IDialog _openFileDialog,
    IBlobFactory _blobFactory
    ) : BaseViewModel
{

    [ObservableProperty]
    public ObservableCollection<BlobModel> _blobs;


    [ObservableProperty]
    public BlobModel _selectedElement;


    [RelayCommand]
    public async Task LoadBlobs()
    {
        try
        {
            var blobCollection = await _blobStorageRepository.GetAllBlobs();

            blobCollection = await Task.WhenAll(blobCollection
                .Select(async blob => await _blobFactory.CreateBlobModel(blob.Name, blob.ContentBase64)));

            Blobs = new ObservableCollection<BlobModel>(blobCollection);
        }
        catch (Exception exc)
        {
            _blobLogger.LogExceptionToTempFile(exc);
            _messageService.Show("An error occurred while loading the files.");
        }
    }


    [RelayCommand]
    public async Task UploadBlob()
    {
        if (_openFileDialog.ShowDialog() != true)
        {
            return;
        }

        string filePath = _openFileDialog.FileName;
        string blobName = _blobFactory.GetBlobName(filePath);

        try
        {
            var uploadedBlob = await _blobStorageRepository.UploadBlob(filePath, blobName);
            uploadedBlob = await _blobFactory.CreateBlobModel(uploadedBlob.Name, uploadedBlob.ContentBase64);
            Blobs.Add(uploadedBlob);
        }
        catch (Exception exc)
        {
            _blobLogger.LogExceptionToTempFile(exc);
        }
    }


    [RelayCommand]
    public async Task DeleteBlob()
    {
        if (!Blobs.Any())
        {
            _messageService.Show("There are no files to delete.");
            return;
        }

        if (SelectedElement is null)
        {
            _messageService.Show("Please select a file to delete.");
            return;
        }

        var delete = _messageService.ShowQuestion("Do you want to permanently delete this file?");

        if (delete is MessageBoxResult.No)
        {
            return;
        }

        try
        {
            await _blobStorageRepository.DeleteBlob(SelectedElement.Name);
            Blobs.Remove(SelectedElement);
        }
        catch (Exception exc)
        {
            _blobLogger.LogExceptionToTempFile(exc);
        }
    }


    public void HandleDeselection() => SelectedElement = null;


    #region Video Player

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VideoNotPlaying))]
    public bool _videoIsPlaying;


    /// <summary>
    /// Determine whether the play button should be displayed.
    /// </summary>
    public bool VideoNotPlaying => !VideoIsPlaying;


    [RelayCommand]
    public void ToggleVideo(object parameter)
    {
        MediaElement medElem = parameter as MediaElement;
        SelectedElement = medElem.DataContext as VideoBlob;

        if (VideoIsPlaying)
        {
            medElem.Pause();
        }
        else
        {
            medElem.Play();
        }

        VideoIsPlaying = !VideoIsPlaying;
    }

    [RelayCommand]
    public void VideoLoaded(object parameter)
    {
        // The video has to be played for a short duration for the thumbnail to be displayed.
        MediaElement medElem = parameter as MediaElement;
        medElem.Volume = 0;
        medElem.Play();
        Thread.Sleep(300);
        medElem.Pause();
        medElem.Volume = 1;
        medElem.Position = TimeSpan.Zero;
    }

    [RelayCommand]
    public void VideoEnded(object parameter)
    {
        (parameter as MediaElement).Stop();
        VideoIsPlaying = false;
        SelectedElement = null;
    }


    #endregion
}