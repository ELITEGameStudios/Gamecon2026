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

    public T Load<T>(string path) where T : class
    {
        T result = JsonUtility.FromJson<T>(path);
        return result;
    }

    public List<T> LoadAll<T>(string path) where T : class
    {
        List<T> results = new ();
        var directory = GetDirectory(path);
        foreach (var item in directory)
        {
            var itemPath = Path.Combine(path, item);
            var contents = File.ReadAllText(itemPath);
            var obj = Load<T>(contents);
            results.Add(obj);
        }
        return results;
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

    public void Write(string path)
    {

    }


}
