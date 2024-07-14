using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TotallyNormalCalculator.MVVM.Model;
internal class UserDiary
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("diaryEntries")]
    public List<DiaryEntryModel> DiaryEntries { get; set; }
}
