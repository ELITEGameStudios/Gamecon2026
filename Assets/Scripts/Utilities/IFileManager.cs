using System.Collections.Generic;
public interface IFileManager
{
    /// <summary>
    /// Provides the string contents of a file.
    /// </summary>
    /// <param name="path"> The file path to read from. </param>
    /// <returns></returns>
    public string Read(string path);

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

    public T ParseFromJson<T>(string contents) where T : class;

    /// <summary>
    /// Provides the files present in a directory, if available.
    /// </summary>
    /// <remarks>Attempts to load all files present in a directory. If the directory doesn't exist, it instead returns <see langword="null"/>. </remarks>
    /// <param name="path">The path of the directory that should be opened.</param>
    /// <returns> Array of <see langword="string"/> made from the files of the directory.     </returns>
    public string[] GetDirectory(string path);
}
