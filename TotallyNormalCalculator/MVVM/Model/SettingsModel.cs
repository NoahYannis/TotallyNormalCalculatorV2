﻿using Newtonsoft.Json;
using System;

namespace TotallyNormalCalculator.MVVM.Model;

internal class SettingsModel : IModel
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("darkmodeactive")]
    public bool DarkModeActive { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; } = "en-US";
}
