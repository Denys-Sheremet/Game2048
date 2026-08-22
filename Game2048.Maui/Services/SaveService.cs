using Game2048.Core.Models;
using Game2048.Core.Serialization;
using Game2048.Maui.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game2048.Maui.Services;

public class SaveService : ISaveService
{
    private readonly string _savePath;
    private readonly string _backupPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger<SaveService> _logger;

    public SaveService(ILogger<SaveService> logger)
    {
        _logger = logger;
        _savePath = Path.Combine(FileSystem.AppDataDirectory, "player_profile.json");
        _backupPath = Path.Combine(FileSystem.AppDataDirectory, "Backups");

        try
        {
            Directory.CreateDirectory(_backupPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create backup directory at {Path}", _backupPath);
        }
        

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { 
                new JsonStringEnumConverter(),
                new NullableIntTupleConverter() 
            }
        };
    }

    public void SaveProfileSync(PlayerProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (!_semaphore.Wait(TimeSpan.FromMilliseconds(500)))
        {
            _logger.LogWarning("SaveProfileSync timed out waiting for semaphore lock");
            return;
        }

        string tempPath = _savePath + ".tmp";

        try
        {
            string jsonString = JsonSerializer.Serialize(profile, _jsonOptions);

            File.WriteAllText(tempPath, jsonString);
            CreateBackup();
            File.Move(tempPath, _savePath, overwrite: true);
            CleanupOldBackups(maxBackups: 5);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute synchronous profile save");
        }
        finally
        {
            TryDeleteTempFile(tempPath);
            _semaphore.Release();
        }
    }


    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

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
            _logger.LogError(ex, "Failed to execute asynchronous profile save");
        }
        finally
        {
            TryDeleteTempFile(tempPath);
            _semaphore.Release();
        }
    }

    public async Task<PlayerProfile?> LoadProfileAsync()
    {
        await _semaphore.WaitAsync();

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
            _logger.LogWarning(ex, "Save file corrupted. Attempting restoration from backup");

            TryDeleteFile(_savePath);
            return await RestoreFromBackupAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error reading profile file");
            return await RestoreFromBackupAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void CleanupOldBackups(int maxBackups = 5)
    {
        try
        {
            DirectoryInfo directory = new DirectoryInfo(_backupPath);
            if (!directory.Exists) return;

            FileInfo[] allBackups = directory.GetFiles("*.bak");

            var sortedBackups = allBackups.OrderByDescending(f => f.Name).ToList();

            if (sortedBackups.Count > maxBackups)
            {
                var filesToDelete = sortedBackups.Skip(maxBackups);

                foreach (var file in filesToDelete)
                {
                    TryDeleteFile(file.FullName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed during old backups cleanup");
        }
    }

    private void CreateBackup() 
    {
        if (!File.Exists(_savePath))
        {
            return;
        }
        try
        {
            Directory.CreateDirectory(_backupPath);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
            string backupFile = Path.Combine(_backupPath, $"profile_{timestamp}.bak");

            File.Copy(_savePath, backupFile, overwrite: true);
        }
        catch (Exception ex) 
        {
            _logger.LogWarning(ex, "Failed to create profile backup");
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
                    return profile;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Backup file {BackupName} is invalid, skipping", backup.Name);
            }
        }

        _logger.LogError("All backup restoration attempts failed");
        return null;
    }

    private bool TryDeleteTempFile(string path)
    {
        if (File.Exists(path))
        {
            return TryDeleteFile(path);
        }
        return false;
    }

    private bool TryDeleteFile(string path)
    {
        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete file at path: {Path}", path);
            return false;
        }
    }
}
