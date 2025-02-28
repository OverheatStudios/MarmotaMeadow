using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "SaveManager", menuName = "Scriptable Objects/SaveManager")]
public class SaveManager : ScriptableObject
{
    /// <summary>
    /// New saves will be created by copying this, should be relative to the assets (data) folder
    /// </summary>
    [SerializeField] private string m_baseSaveName;

    private const string TEST_SAVE = "Test";

    /// <summary>
    /// Where are saves stored?
    /// </summary>
    [SerializeField] private string m_savesDirectoryName;

    /// <summary>
    /// Name of current save folder
    /// </summary>
    private string m_currentSaveName;

    private void OnEnable()
    {
        if (!GameRunning.IsGameRunning()) return;
        m_currentSaveName = TEST_SAVE;

        if (!Directory.Exists(GetBaseSaveDirectoryPath()))
        {
            Directory.CreateDirectory(GetBaseSaveDirectoryPath());
        }

        if (!Directory.Exists(GetSavesDirectoryPath()))
        {
            Directory.CreateDirectory(GetSavesDirectoryPath());
        }

        SwitchSave(m_currentSaveName);
        if (!Directory.Exists(GetCurrentSavePath()))
        {
            CreateSave(GetCurrentSavePath());
        }
    }

    public void NewSave()
    {
        // Are we already in a save
        if (m_currentSaveName != TEST_SAVE)
        {
            Debug.LogWarning("Created a new save when the current save was already set (this warning might mean nothing)");
        }

        // Generate new save name
        string newSaveName = "";
        string[] saves = GetSaves();
        for (int i = 0; i <= saves.Length; ++i)
        {
            if (saves.Contains(i + "")) continue;
            newSaveName = i + "";
            break;
        }
        Assert.IsFalse(newSaveName == "");

        // Create save
        CreateSave(Path.Combine(GetSavesDirectoryPath(), newSaveName));
        SwitchSave(newSaveName);
    }

    public void SwitchSave(string saveName)
    {
        m_currentSaveName = saveName;
    }

    /// <summary>
    /// Get path to a file in the current save
    /// </summary>
    /// <param name="fileName">File name, e.g NightInventory.json or test/something.json</param>
    /// <returns>Absolute path to the file for the current save, doesn't guarantee the file exists</returns>
    public string GetFilePath(string fileName)
    {
        return Path.Combine(GetCurrentSavePath(), fileName);
    }

    public string[] GetSaves()
    {
        string[] fullPaths = Directory.GetDirectories(GetSavesDirectoryPath());
        string[] saves = new string[fullPaths.Length];
        for (int i = 0; i < fullPaths.Length; ++i)
        {
            string folder = fullPaths[i];
            saves[i] = Path.GetFileName(folder);
        }
        return saves;
    }

    /// <summary>
    /// Create a save
    /// </summary>
    /// <param name="fullSavePath">The save path, should be absolute</param>
    private void CreateSave(string fullSavePath)
    {
        CopyFolderOrFileNoMeta(GetBaseSaveDirectoryPath(), fullSavePath);
    }

    private string GetCurrentSavePath()
    {
        return Path.Combine(GetSavesDirectoryPath(), m_currentSaveName);
    }

    private string GetBaseSaveDirectoryPath()
    {
        return Path.Combine(Application.dataPath, m_baseSaveName);
    }

    private string GetSavesDirectoryPath()
    {
        return Path.Combine(Application.dataPath, m_savesDirectoryName);
    }

    /// <summary>
    /// Recursively copies all files and folders to another destination, this will not copy any files ending in .meta.
    /// This will copy empty directories.
    /// This function only works inside Application.dataPath for safety reasons, don't want to accidentally copy the whole c drive or something.
    /// </summary>
    /// <param name="path">Path of folder to copy</param>
    /// <param name="dest">Destination to copy to, make sure there isn't anything here already</param>
    private void CopyFolderOrFileNoMeta(string path, string dest)
    {
        // Safety check
        if (!IsPathInside(path, Application.dataPath) || !IsPathInside(dest, Application.dataPath))
        {
            Assert.IsTrue(false);
            return;
        }
        if (path == dest) return;

        // Copy file
        if (File.Exists(path))
        {
            if (!path.EndsWith(".meta"))
            {
                File.Copy(path, dest);
            }
            return;
        }

        // Copy directory
        if (!Directory.Exists(path)) return;

        if (!Directory.Exists(dest))
        {
            Directory.CreateDirectory(dest);
        }

        // Copy all files and directories in this directory
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file); // bad engine decides "name of file" that GetFiles returns means absolute path because that makes sense (im mad)
            CopyFolderOrFileNoMeta(Path.Combine(path, name), Path.Combine(dest, name));
        }

        string[] folders = Directory.GetDirectories(path);
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            CopyFolderOrFileNoMeta(Path.Combine(path, name), Path.Combine(dest, name));
        }
    }

    /// <summary>
    /// Check if a path is inside another path, examples:
    ///
    /// path = a/b/c.json
    /// directory = a/
    /// output = true because path is inside directory
    ///
    /// path = a/b/c.json
    /// directory = a / d /
    /// output = false because path isnt inside directory
    /// </summary>
    /// <param name="path"></param>
    /// <param name="directory"></param>
    /// <returns>True if path is inside directory</returns>
    private bool IsPathInside(string path, string directory)
    {
        string fullPath = Path.GetFullPath(path).Replace("\\", "/");
        string dirPath = Path.GetFullPath(directory).Replace("\\", "/");

        if (!dirPath.EndsWith("/")) dirPath += "/";

        return fullPath.StartsWith(dirPath, StringComparison.OrdinalIgnoreCase);
    }
}
