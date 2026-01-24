using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IFileManager
{
    public string Read(string path);
    public void Write(string path);
    public bool Save(string path, string name, object contents);
    /// <summary>
    /// Ensures that the specified content is saved to the given file path.
    /// </summary>
    /// <remarks>This method attempts to save the provided content to the specified file path. If the
    /// operation fails  (e.g., due to insufficient permissions or an invalid path), it returns <see langword="false"/>
    /// instead  of throwing an exception.</remarks>
    /// <param name="path">The file path where the content should be saved. If it doesn't exist, the directory will be made.</param>
    /// <param name="contents">The content to be written to the file. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="name"> The name of the file being created.</param>
    /// <returns><see langword="true"/> if the content was successfully saved; otherwise, <see langword="false"/>.</returns>
    public bool EnsureSave(string path, string name, object contents); 
    public T Load<T>(string path) where T : class;

    public List<T> LoadAll<T>(string path) where T : class;

    public string[] GetDirectory(string path);
}
