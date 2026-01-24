using System.Collections.Generic;
using System.IO;
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


    public T ParseFromJson<T>(string contents) where T : class
    {
        return JsonUtility.FromJson<T>(contents);
    }


    public string Read(string path)
    {
        string fileContents = File.ReadAllText(path);
        return fileContents;
    }

    public bool Save(string path, string name, object contents)
    {
        if (contents == null || GetDirectory(path) == null) return false;
        string newFile = JsonUtility.ToJson(contents);
        string fileDestination = Path.Combine(path, name + ".json");
        File.WriteAllText(fileDestination, newFile);
        return true;
    }

    public bool EnsureSave(string path, string name, object contents)
    {
        if (contents == null) return false;
        if (!Directory.Exists (path))
        {
            Directory.CreateDirectory(path);
        }
        string newFile = JsonUtility.ToJson(contents);
        string fileDestination = Path.Combine(path, name + ".json");
        File.WriteAllText(fileDestination, newFile);
        return true;
    }


}
