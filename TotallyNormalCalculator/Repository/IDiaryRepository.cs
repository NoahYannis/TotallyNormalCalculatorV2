using System.Collections.Generic;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository;
public interface IDiaryRepository
{
    public IEnumerable<DiaryEntryModel> GetAllDiaryEntries();
    public DiaryEntryModel GetDiaryEntryById(int id);
    public void AddDiaryEntry(DiaryEntryModel diaryEntry);
    public void UpdateDiaryEntry(DiaryEntryModel diaryEntry);
    public void DeleteDiaryEntry(int id);
}
