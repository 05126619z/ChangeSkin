using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace ChangeSkin;

internal static class SkinLoader
{
    // public static IEnumerator LoadSkinAsync(
    //     string skinName,
    //     string[] filenames,
    //     bool isLocal,
    //     Action<Dictionary<string, Sprite>> callback
    // )
    // {
    //     Dictionary<string, Sprite> dict = new();
    //     string workPath;
    //     if (isLocal)
    //     {
    //         UpdateFromLocal(skinName);
    //         workPath = Path.GetTempPath() + $"/ChangeSkin/local/{skinName}";
    //     }
    //     else
    //     {
    //         workPath = Path.GetTempPath() + $"/ChangeSkin/remote/{skinName}";
    //         string skinDir = Directory.GetDirectories(workPath)[0];
    //         workPath = workPath + "/" + skinDir;
    //     }
    //     foreach (string filename in filenames)
    //     {
    //         Sprite sprite = Utils.LoadSprite(workPath + "/" + filename);
    //         sprite.name = Path.GetFileNameWithoutExtension(filename);
    //         dict[Path.GetFileNameWithoutExtension(filename)] = sprite;
    //         yield return null;
    //     }
    //     callback?.Invoke(dict);
    // }

    public static void LoadSkin(
        string skinName,
        string[] filenames,
        bool isLocal,
        ref Dictionary<string, Sprite> dict
    )
    {
        string workPath;
        dict = [];
        if (isLocal)
        {
            UpdateLocalSkin(skinName);
            workPath = Path.GetTempPath() + $"/ChangeSkin/local/{skinName}";
        }
        else
        {
            workPath = Path.GetTempPath() + $"/ChangeSkin/remote/{skinName}";
            workPath = Directory.GetDirectories(workPath).Single();
        }
        foreach (string filename in filenames)
        {
            Sprite sprite = Utils.LoadSprite(workPath + "/Textures/" + filename);
            sprite.name = Path.GetFileNameWithoutExtension(filename);
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

    public static string UploadLocal(string skinName, string uploadUrl)
    {
        string workPath = Path.GetTempPath() + "/ChangeSkin/zips/local";
        string filePath = workPath + $"/{skinName}.zip";
        Uri uri = new(uploadUrl);
        byte[] fileBytes = File.ReadAllBytes(filePath);
        using (HttpClient client = new HttpClient())
        using (var formData = new MultipartFormDataContent())
        {
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                "application/zip"
            );
            formData.Add(fileContent, "file", Path.GetFileName(filePath));
            HttpResponseMessage response = client.PostAsync(uploadUrl, formData).Result;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                string result = response.Headers.GetValues("Location").FirstOrDefault();
                Plugin.Instance.Logger.LogInfo(result);
                return result;
            }
            else
            {
                Plugin.Instance.Logger.LogWarning("Wrong responce: " + response.Content.ReadAsStringAsync().Result);
            }
        }
        return null;
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
                using (
                    var fs = new FileStream(
                        workPath + "/" + fileName + ".zip",
                        FileMode.OpenOrCreate
                    )
                )
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
        if (!Directory.Exists(workPath + "/" + skinName))
            Directory.CreateDirectory(workPath + "/" + skinName);
        ZipFile.ExtractToDirectory(
            Path.GetTempPath() + $"/ChangeSkin/zips/remote/{archiveName}.zip",
            workPath + "/" + skinName
        );
        // if (Directory.Exists($"{workPath}/{skinName}"))
        //     Directory.Delete($"{workPath}/{skinName}", true);
    }

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
