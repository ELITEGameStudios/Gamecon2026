using System;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveSystem : IFileManager
{
    public string[] GetDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path);
        }
        return null;
    }

    public int GetNumberOfFilesInDirectory(string path)
    {
        if (!Directory.Exists(path)) return -1;

        return Directory.GetFiles(path).Length;
    }
    public T ParseFromJson<T>(string contents) where T : class
    {
        try
        {
            return JsonUtility.FromJson<T>(contents);
        } 
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }
    public string Read(string path)
    {
        if (!File.Exists(path)) return string.Empty;
        try
        {
            string fileContents = File.ReadAllText(path);
            return fileContents;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    public bool Save(string path, string name, object contents)
    {
        if (contents == null || GetDirectory(path) == null) return false;
        string newFile = JsonUtility.ToJson(contents);
        int fileSize = Encoding.UTF8.GetByteCount(newFile.AsSpan());
        if (!HasEnoughSpace(fileSize)) return false;
        string fileDestination = Path.Combine(path, name + ".json");
        try
        {
            File.WriteAllText(fileDestination, newFile);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return false;
    }

    bool HasEnoughSpace(long size)
    {
        DriveInfo c = new (Application.persistentDataPath);
        if (c != null)
        {
            return c.AvailableFreeSpace > size;
        }
        return false;
    }

    public bool EnsureSave(string path, string name, object contents)
    {
        if (contents == null) return false;
        if (!Directory.Exists (path)) Directory.CreateDirectory(path);
        string newFile = JsonUtility.ToJson(contents);
        string fileDestination = Path.Combine(path, name + ".json");
        File.WriteAllText(fileDestination, newFile);
        return true;
    }
}