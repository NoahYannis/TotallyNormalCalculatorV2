using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.MVVM.ViewModels;
internal partial class BlobStorageViewModel(ITotallyNormalCalculatorLogger _blobLogger,
    IBlobStorageRepository<BlobModel> _blobStorageRepository) : BaseViewModel
{

    [ObservableProperty]
    private ObservableCollection<BlobModel> _blobs;


    [ObservableProperty]
    private BlobModel _selectedElement;


    [RelayCommand]
    public async Task LoadBlobs()
    {
        try
        {
            var blobCollection = await _blobStorageRepository.GetAllBlobs();
            Blobs = new ObservableCollection<BlobModel>(blobCollection);
        }
        catch (Exception exc)
        {
            _blobLogger.LogExceptionToTempFile(exc);
            MessageBox.Show("An error occurred while loading the files.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    [RelayCommand]
    public async Task UploadBlob()
    {
        var openFileDialog = new OpenFileDialog()
        {
            Title = "Select a file to upload",
        };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        string filePath = openFileDialog.FileName;
        string blobName = Path.GetFileName(filePath);

        try
        {
            var uploadedBlob = await _blobStorageRepository.UploadBlob(filePath, blobName);
            Blobs.Add(uploadedBlob);
        }
        catch (Exception exc)
        {
            _blobLogger.LogExceptionToTempFile(exc);
        }
    }

    [RelayCommand]
    public async Task ToggleVideo(object o)
    {
        MessageBox.Show("Hi");
    }

    [RelayCommand]
    public async Task DeleteBlob()
    {
        if (!Blobs.Any())
        {
            MessageBox.Show("There are no files to delete.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (SelectedElement is null)
        {
            MessageBox.Show("Please select a file to delete.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var delete = MessageBox.Show("Do you want to permanently delete this file?", "TotallyNormalCalculator",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (delete is MessageBoxResult.Yes)
        {
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
    }

    public void HandleDeselection()
    {
        SelectedElement = null;
    }
}