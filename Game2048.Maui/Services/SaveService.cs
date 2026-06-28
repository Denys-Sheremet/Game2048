using Game2048.Core.Models;
using Game2048.Maui.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Game2048.Maui.Services;

public class SaveService : ISaveService
{
    private readonly string _savePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public SaveService()
    {
        _savePath = Path.Combine(FileSystem.AppDataDirectory, "player_profile.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
        };
    }

    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(profile, _jsonOptions);
            await File.WriteAllTextAsync(_savePath, jsonString);
        }
        catch (Exception ex)
        {
            //log
            Debug.WriteLine(ex);
            //need to delete
        }
    }

    public async Task<PlayerProfile?> LoadProfileAsync()
    {
        try
        {
            if (!File.Exists(_savePath))
            {
                return null;
            }

            string jsonString = await File.ReadAllTextAsync(_savePath);
            return JsonSerializer.Deserialize<PlayerProfile>(jsonString, _jsonOptions);
        }
        catch (Exception ex)
        {
            //log
            Debug.WriteLine(ex);
            //need to delete
            return null;
        }
    }
}
