using Game2048.Core.Enums;
using Game2048.Core.Models;
using Game2048.Core.Serialization;
using Game2048.Maui.Interfaces;
using Game2048.Maui.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game2048.Maui.Services;

public class SaveService : ISaveService
{
    private readonly string _savePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IProfileManager _profileManager;

    public SaveService(IProfileManager profileManager)
    {
        _savePath = Path.Combine(FileSystem.AppDataDirectory, "player_profile.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { 
                new JsonStringEnumConverter(),
                new NullableIntTupleConverter() 
            }
        };
        _profileManager = profileManager;
    }

    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        string tempPath = _savePath + ".tmp";

        try
        {
            string jsonString = JsonSerializer.Serialize(profile, _jsonOptions);

            await File.WriteAllTextAsync(tempPath, jsonString);

            File.Move(tempPath, _savePath, overwrite: true);
        }
        catch (Exception ex)
        {
            #if DEBUG
            Debug.WriteLine($"Error while saving profile: {ex.Message}");
            #else
            #endif
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (Exception cleanupEx)
                {
                    Debug.WriteLine($"Error while deleting temporary file: {cleanupEx.Message}");
                }
            }
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
            Debug.WriteLine(jsonString);//
            Debug.WriteLine(jsonString.Length);//
            return JsonSerializer.Deserialize<PlayerProfile>(jsonString, _jsonOptions);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Corrupted save file: {ex.Message}");
            try { File.Delete(_savePath); } catch { }
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine($"Error while loading profile: {ex.Message}");
            return null;
        }
    }
}
