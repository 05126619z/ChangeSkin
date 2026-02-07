using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace ChangeSkin;

internal static class SkinLoader
{
    public static IEnumerator LoadSkinAsync(
        string skinName,
        string[] filenames,
        bool isLocal,
        Action<Dictionary<string, Sprite>> callback
    )
    {
        Dictionary<string, Sprite> dict = new();
        foreach (string filename in filenames)
        {
            string workPath;
            if (isLocal)
            {
                UpdateFromLocal(skinName);
                workPath = Path.GetTempPath() + $"/ChangeSkin/local/{skinName}/Textures/{filename}";
            }
            else
            {
                workPath = Path.GetTempPath() + $"/ChangeSkin/remote/{skinName}/Textures/{filename}";
            }
            Sprite sprite = Utils.LoadSprite(workPath);
            sprite.name=Path.GetFileNameWithoutExtension(filename);
            dict[Path.GetFileNameWithoutExtension(filename)] = sprite;
            yield return null;
        }
        callback?.Invoke(dict);
    }

    public static void LoadSkin(
        string skinName,
        string[] filenames,
        bool isLocal,
        ref Dictionary<string, Sprite> dict
    )
    {
        foreach (string filename in filenames)
        {
            string workPath;
            if (isLocal)
            {
                UpdateFromLocal(skinName);
                workPath = Path.GetTempPath() + $"/ChangeSkin/local/{skinName}/Textures/{filename}";
            }
            else
            {
                workPath = Path.GetTempPath() + $"/ChangeSkin/remote/{skinName}/Textures/{filename}";
            }
            Sprite sprite = Utils.LoadSprite(workPath);
            sprite.name=Path.GetFileNameWithoutExtension(filename);
            dict[Path.GetFileNameWithoutExtension(filename)] = sprite;
        }
        }

    public static void UpdateLocalSkin(string skinName)
    {
        UpdateFromLocal(skinName);
        PackLocal(skinName);
    }

    private static void UpdateFromLocal(string skinName)
    {
        string workPath = Path.GetTempPath() + "/ChangeSkin/local";
        if (!Directory.Exists(workPath))
            Directory.CreateDirectory(workPath);
        if (Directory.Exists($"{workPath}/{skinName}"))
            Directory.Delete($"{workPath}/{skinName}", true);
        CopyFolderRecuresively(
            Paths.PluginPath + $"/ChangeSkin/resources/{skinName}",
            $"{workPath}/{skinName}"
        );
    }

    private static void PackLocal(string skinName)
    {
        string workPath = Path.GetTempPath() + "/ChangeSkin/zips/local";
        if (!Directory.Exists(workPath))
            Directory.CreateDirectory(workPath);
        if (File.Exists($"{workPath}/{skinName}.zip"))
            File.Delete($"{workPath}/{skinName}.zip");
        ZipFile.CreateFromDirectory(
            Path.GetTempPath() + $"/ChangeSkin/local/{skinName}",
            $"{workPath}/{skinName}.zip"
        );
    }

    public static string DownloadRemote(string url)
    {
        string workPath = Path.GetTempPath() + "/ChangeSkin/zips/remote";
        if (!Directory.Exists(workPath))
            Directory.CreateDirectory(workPath);
        string fileName = Path.GetFileName(url);
        if (string.IsNullOrEmpty(fileName))
        {
            throw (new Exception("ChangeSkin invalid url, no filename"));
        }

        string filePath = Path.Combine(workPath, fileName);

        // Download using WebClient (simplest approach)
        // simplest approach my ass deepseek
        using (var client = new HttpClient())
        {
            using (var s = client.GetStreamAsync(url).GetAwaiter().GetResult())
            {
                using (var fs = new FileStream(workPath + "/" + fileName+".zip", FileMode.OpenOrCreate))
                {
                    s.CopyTo(fs);
                }
            }
        }

        return Path.GetFileNameWithoutExtension(fileName);
    }

    public static void UnpackRemote(string skinName, string archiveName)
    {
        string workPath = Path.GetTempPath() + "/ChangeSkin/remote";
        if (!Directory.Exists(workPath))
            Directory.CreateDirectory(workPath);
        if (Directory.Exists($"{workPath}/{skinName}"))
            Directory.Delete($"{workPath}/{skinName}", true);
        ZipFile.ExtractToDirectory(Path.GetTempPath() + $"/ChangeSkin/zips/remote/{archiveName}.zip", workPath);
    }

    // public static void UploadLocal(string skinName)
    // {
    //     string workPath = Path.GetTempPath() + "/ChangeSkin/zips/local";
    //     string filePath = workPath + $"{skinName}.zip";

    //     MultipartFormDataContent content = new();
    //     byte[] signature = Plugin.ModConfig.SignFile(filePath);
    //     byte[] zipBytes = File.ReadAllBytes(filePath);

    //     ByteArrayContent fileContent = new ByteArrayContent(zipBytes);
    //     fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
    //         "application/zip"
    //     );

    //     string signatureBase64 = Convert.ToBase64String(signature);
    //     content.Add(new StringContent(signatureBase64, Encoding.UTF8), "signature");

    //     byte[] publicKey = Plugin.ModConfig.GetPublicKey();
    //     string publicKeyBase64 = Convert.ToBase64String(publicKey);
    //     content.Add(new StringContent(publicKeyBase64, Encoding.UTF8), "publicKey");
    // }

    private static void CopyFolderRecuresively(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }
        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            string dest = Path.Combine(destFolder, name);
            CopyFolderRecuresively(folder, dest);
        }
    }

    private static void UnpackZip(string skinName)
    {
        if (!File.Exists(Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip"))
            return;
        if (Directory.Exists(Path.GetTempPath() + $"/ChangeSkin/{skinName}"))
            Directory.Delete(Path.GetTempPath() + $"/ChangeSkin/{skinName}", true);
        ZipFile.ExtractToDirectory(
            Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip",
            Path.GetTempPath() + $"/ChangeSkin/{skinName}"
        );
    }

    private static void CreateZip(string path, string skinName)
    {
        string outPath;
        outPath = Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip";
        if (!Directory.Exists(Path.GetTempPath() + "/ChangeSkin"))
            Directory.CreateDirectory(Path.GetTempPath() + "/ChangeSkin");
        if (File.Exists(outPath))
            File.Delete(outPath);
        ZipFile.CreateFromDirectory(path, outPath);
    }
}
