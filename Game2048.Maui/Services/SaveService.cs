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
    private readonly string _backupPath;
    private readonly JsonSerializerOptions _jsonOptions;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public SaveService()
    {
        _savePath = Path.Combine(FileSystem.AppDataDirectory, "player_profile.json");
        _backupPath = Path.Combine(FileSystem.AppDataDirectory, "Backups");
        Directory.CreateDirectory(_backupPath);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { 
                new JsonStringEnumConverter(),
                new NullableIntTupleConverter() 
            }
        };
    }

    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        await _semaphore.WaitAsync();

        string tempPath = _savePath + ".tmp";

        try
        {
            string jsonString = JsonSerializer.Serialize(profile, _jsonOptions);
            await File.WriteAllTextAsync(tempPath, jsonString);

            CreateBackup();

            File.Move(tempPath, _savePath, overwrite: true);

            CleanupOldBackups(maxBackups: 5);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"Error while saving profile: {ex.Message}");
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
#if DEBUG
                    Debug.WriteLine($"Error while deleting temporary file: {cleanupEx.Message}");
#endif
                    throw; //throw the exception to be handled by the caller if needed
                }
            }
            _semaphore.Release();
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
        catch (JsonException ex)
        {
#if DEBUG
            Debug.WriteLine($"Corrupted save file: {ex.Message}");
#endif
            try { File.Delete(_savePath); } catch { }
            return await RestoreFromBackupAsync();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine($"Error while loading profile: {ex.Message}");
#endif
            try { File.Delete(_savePath); } catch { }
            return await RestoreFromBackupAsync();
        }
    }

    private void CleanupOldBackups(int maxBackups = 5)
    {
        DirectoryInfo directory = new DirectoryInfo(_backupPath);

        FileInfo[] allBackups = directory.GetFiles("*.bak");

        var sortedBackups = allBackups.OrderByDescending(f => f.Name).ToList();

        if (sortedBackups.Count > maxBackups)
        {
            var filesToDelete = sortedBackups.Skip(maxBackups);

            foreach (var file in filesToDelete)
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    //if deletion fails, continue
                }
            }
        }
    }

    private void CreateBackup() 
    {
        if (File.Exists(_savePath))
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFile = Path.Combine(_backupPath, $"profile_{timestamp}.bak");
            string oldSaveContent = File.ReadAllText(_savePath);
            File.WriteAllText(backupFile, oldSaveContent);
        }
    }

    private async Task<PlayerProfile?> RestoreFromBackupAsync()
    {
        DirectoryInfo directory = new DirectoryInfo(_backupPath);
        if (!directory.Exists) return null;

        var backups = directory.GetFiles("*.bak").OrderByDescending(f => f.Name).ToList();

        foreach (var backup in backups)
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync(backup.FullName);
                var profile = JsonSerializer.Deserialize<PlayerProfile>(jsonString, _jsonOptions);

                if (profile is not null)
                {
                    await File.WriteAllTextAsync(_savePath, jsonString);
#if DEBUG
                    Debug.WriteLine($"Successfully restored from {backup.Name}");
#endif
                    return profile;
                }
            }
            catch
            {
                //just continue to the next backup if this one fails
            }
        }

        //if all backups fail, return null
        return null;
    }
}
