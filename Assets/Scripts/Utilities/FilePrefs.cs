using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class FilePrefs
{
    public string rootPath;

    public Dictionary<string, string> folders { get; } = new Dictionary<string, string>();

    public string currentFolder = "";

    public FilePrefs(string rootPath)
    {
        this.rootPath = rootPath;
    }

    public string FullPath() => FullPath("");
    public string FullPath(string path)
    {
        if (currentFolder != "")
        {
            var folderPath = folders[currentFolder];
            return Path.Join(rootPath, folderPath, path);
        }
        return Path.Join(rootPath, path);
    }

    public FilePrefs Folder(string key, string path)
    {
        folders.Add(key, path);
        Directory.CreateDirectory(FullPath(path));
        return this;
    }

    public FilePrefs CreateDirectory(string path)
    {
        Directory.CreateDirectory(FullPath(path));
        return this;
    }

    public FilePrefs In(string folderKey)
    {
        currentFolder = folderKey;
        return this;
    }
    public FilePrefs Root()
    {
        currentFolder = "";
        return this;
    }

    public void Write(string path, byte[] bytes)
    {
        File.WriteAllBytes(FullPath(path), bytes);
    }
    public async Task WriteAsync(string path, byte[] bytes)
    {
        await File.WriteAllBytesAsync(FullPath(path), bytes);
    }

    public void WriteText(string path, string data)
    {
        string _path = FullPath(path);
        var stream = File.CreateText(_path);
        stream.Write(data);
        stream.Close();
    }
    public void WriteTextAsync(string path, string data)
    {
        string _path = FullPath(path);
        var stream = File.CreateText(_path);
        stream.WriteAsync(data);
        stream.Close();
    }

    public byte[] Read(string path)
    {
        return File.ReadAllBytes(FullPath(path));
    }
    public async Task<byte[]> ReadAsync(string path)
    {
        return await File.ReadAllBytesAsync(FullPath(path));
    }

    public string ReadText(string path)
    {
        try
        {
            string _path = FullPath(path);
            if (File.Exists(_path))
            {
                var data = File.ReadAllText(_path);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return "";
    }
    public async Task<string> ReadTextAsync(string path)
    {
        try
        {
            string _path = FullPath(path);
            if (File.Exists(_path))
            {
                var data = await File.ReadAllTextAsync(_path);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return "";
    }

    public bool Exist(string path)
    {
        return File.Exists(FullPath(path));
    }
    public void Delete(string path)
    {
        File.Delete(FullPath(path));
    }

    public string[] GetAllDirectories(string dir)
    {
        string path = FullPath(dir);
        DirectoryInfo d = new DirectoryInfo(path);
        FileInfo[] fs = d.GetFiles();
        var dirs = new string[fs.Length];
        for (int i = 0; i < fs.Length; i++)
        {
            dirs[i] = fs[i].Name;
        }
        return dirs;
    }

    public void WriteTexture2D(string path, Texture2D texture)
    {
        File.WriteAllBytes(FullPath(path), WriteTexture2D(texture));
    }
    public Texture2D ReadTexture2D(string path)
    {
        byte[] bytes = File.ReadAllBytes(FullPath(path));
        return ReadTexture2D(bytes);
    }
    public async Task WriteTexture2DAsync(string path, Texture2D texture)
    {
        await File.WriteAllBytesAsync(FullPath(path), WriteTexture2D(texture));
    }
    public async Task<Texture2D> ReadTexture2DAsync(string path)
    {
        byte[] bytes = await File.ReadAllBytesAsync(FullPath(path));
        return ReadTexture2D(bytes);
    }

    private byte[] WriteTexture2D(Texture2D texture)
    {
        return texture.EncodeToPNG();
    }
    private Texture2D ReadTexture2D(byte[] bytes)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        return texture;
    }
}
