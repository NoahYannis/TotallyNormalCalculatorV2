using Microsoft.Win32;

namespace TotallyNormalCalculator;
public class TncOpenFileDialog : IDialog
{
    private readonly OpenFileDialog _openFileDialog;

    public string FileName
    {
        get => _openFileDialog.FileName;
        set => _openFileDialog.FileName = value;
    }

    public bool? ShowDialog() => _openFileDialog.ShowDialog();
}
