using CommunityToolkit.Mvvm.ComponentModel;

namespace TotallyNormalCalculator.MVVM.Model;

public class DiaryEntryModel : ObservableObject
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Date { get; set; }

}
