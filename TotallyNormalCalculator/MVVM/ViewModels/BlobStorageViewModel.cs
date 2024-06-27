using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TotallyNormalCalculator.Logging;
using TotallyNormalCalculator.MVVM.Model;
using TotallyNormalCalculator.Repository.BlobStorage;

namespace TotallyNormalCalculator.MVVM.ViewModels;
internal partial class BlobStorageViewModel : BaseViewModel
{
    private readonly ITotallyNormalCalculatorLogger _blobLogger;
    private readonly IBlobStorageRepository<BlobModel> _blobStorageRepository;


    [ObservableProperty]
    private ObservableCollection<BlobModel> _blobs = [];

    [ObservableProperty]
    private BlobModel _selectedBlob;

    public BlobStorageViewModel(ITotallyNormalCalculatorLogger logger,
        IBlobStorageRepository<BlobModel> blobStorageRepository)
    {
        (_blobLogger, _blobStorageRepository) = (logger, blobStorageRepository);
        Blobs = Task.Run(() => this.GetAllBlobs()).GetAwaiter().GetResult();
    }


    [RelayCommand]
    public async Task<ObservableCollection<BlobModel>> GetAllBlobs()
    {
        var blobs = await _blobStorageRepository.GetAllBlobs();
        return new ObservableCollection<BlobModel>(blobs);
    }


    [RelayCommand]
    public async Task UploadBlob()
    {
         var openFileDialog = new OpenFileDialog()
        {
            Title = "Select a file to upload",
        };

        if (openFileDialog.ShowDialog() !=  true)
        {
            return;
        }

        string filePath = openFileDialog.FileName;
        string blobName = Path.GetFileName(filePath);

        await _blobStorageRepository.UploadBlob(filePath, blobName);
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

        if (SelectedBlob is null)
        {
            MessageBox.Show("Please select a file to delete.", "TotallyNormalCalculator",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var delete = MessageBox.Show("Do you want to permanently delete this file?", "TotallyNormalCalculator",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (delete is MessageBoxResult.Yes)
        {
            await _blobStorageRepository.DeleteBlob(SelectedBlob.Name);
        }
    }
}