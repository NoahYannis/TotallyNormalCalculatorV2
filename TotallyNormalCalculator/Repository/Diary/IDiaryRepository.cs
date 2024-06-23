using System.Collections.Generic;
using System.Threading.Tasks;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Repository;
public interface IDiaryRepository
{
    public Task<IEnumerable<DiaryEntryModel>> GetAllDiaryEntries();
    public Task AddDiaryEntry(DiaryEntryModel diaryEntry);
    public Task UpdateDiaryEntry(DiaryEntryModel diaryEntry);
    public Task DeleteDiaryEntry(int id);
}
