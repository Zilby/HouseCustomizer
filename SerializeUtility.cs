using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SerializeUtility<T> where T : new()
{
    public const string EXTENSION = ".zlb";

    public static T Load(string filename, string directory)
    {
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        string path = GetPath(directory, filename);
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
#if UNITY_EDITOR
            T data = new T();
            object boxed = data;
            EditorJsonUtility.FromJsonOverwrite(dataAsJson, boxed);
            data = (T)boxed;
#else
            T data = JsonUtility.FromJson<T>(dataAsJson);
#endif
            return data;
        }
        else
        {
            Debug.LogWarning("Failed to deserialize save file: file not found.");
        }
        return default(T);
    }

    public static void Write(T t, string fileName, string directory)
    {
        Debug.LogFormat("Writing out {0}", fileName);
#if UNITY_EDITOR
        string dataAsJson = EditorJsonUtility.ToJson(t, true);
#else
        string dataAsJson = JsonUtility.ToJson(t, true);
#endif
        File.WriteAllText(GetPath(directory, fileName), dataAsJson);
        Debug.LogFormat("Finished writing {0}", fileName);
    }

    /// <summary>
    /// Returns the filepath as a string.
    /// </summary>
    public static string GetPath(string directory, string filename)
    {
        string path = CombinePaths(new string[] { "Assets", "Standard Assets", "Resources", directory, filename });
        // If the filename does not end with .xml, add it.
        if (!filename.EndsWith(EXTENSION))
        {
            path += EXTENSION;
        }
        return path;
    }

    /// <summary>
    /// Combines multiple paths.
    /// </summary>
    /// <returns>The path.</returns>
    public static string CombinePaths(string[] strings)
    {
        string path = "";
        foreach (string s in strings)
        {
            path = Path.Combine(path, s);
        }
        return path;
    }

    public static string[] GetFiles(string directory)
    {
        string path = CombinePaths(new string[] { Application.dataPath, "Standard Assets", "Resources", directory });
        DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
        FileInfo[] files = d.GetFiles("*" + EXTENSION); //Getting Text files
        string[] fileNames = new string[files.Length];
        for (int i = 0; i < files.Length; ++i)
        {
            fileNames[i] = files[i].Name.Substring(0, files[i].Name.Length - EXTENSION.Length);
        }
        return fileNames;
    }
}
