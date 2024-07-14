using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;


namespace TotallyNormalCalculator.MVVM.Model;


public class DiaryEntryModel : ObservableObject, IModel
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }
}
