﻿using System;
using System.IO;

namespace TotallyNormalCalculator.Helper;

/// <summary>
///  Load application secrets into memory from the.env file.
/// </summary>
public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The .env file at path '{filePath}' does not exist.");


        foreach (var line in File.ReadAllLines(filePath))
        {
            var keyIndex = line.IndexOf("=");
            var s = line.IndexOf("#");
            var key = line.Split('=')[0];
            var value = line.Substring(keyIndex + 1, line.IndexOf("#") - (keyIndex + 1));

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
