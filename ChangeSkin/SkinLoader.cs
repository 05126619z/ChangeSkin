using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using BepInEx;
using UnityEngine;

namespace ChangeSkin;

internal static class SkinLoader
{
    public static void LoadFromFolder(
        string skinName,
        string[] filenames,
        ref Dictionary<string, Sprite> dict
    )
    {
        if (!File.Exists(Path.GetTempPath() + $"/ChangeSkin/{skinName}"))
            SaveLocalSkin(skinName);
        foreach (string filename in filenames)
        {
            string path =
                Paths.PluginPath + $"/ChangeSkin/resources/{skinName}/Textures/{filename}";
            Sprite sprite;
            try
            {
                sprite = Utils.LoadSprite(path);
            }
            catch (Exception e)
            {
                throw (e);
            }

            string formattedFilename = Path.GetFileNameWithoutExtension(filename);
            sprite.name = formattedFilename;
            dict.Add(formattedFilename, sprite);
        }
    }

    public static void LoadFromURL() { }

    public static void LoadFromClient() { }

    public static void SaveLocalSkin(string skinName)
    {
        string path = Paths.PluginPath + $"/ChangeSkin/resources/{skinName}";
        CreateZip(path, skinName);
        UnpackZip(skinName);
    }

    private static void UnpackZip(string skinName)
    {
        if (!File.Exists(Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip")) return;
        if (Directory.Exists(Path.GetTempPath() + $"/ChangeSkin/{skinName}")) Directory.Delete(Path.GetTempPath() + $"/ChangeSkin/{skinName}", true);
        ZipFile.ExtractToDirectory(Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip", Path.GetTempPath() + $"/ChangeSkin/{skinName}");
    }

    private static void CreateZip(string path, string skinName)
    {
        string outPath;
        outPath = Path.GetTempPath() + $"/ChangeSkin/{skinName}.zip";
        if (!Directory.Exists(Path.GetTempPath() + "/ChangeSkin")) Directory.CreateDirectory(Path.GetTempPath() + "/ChangeSkin");
        if (File.Exists(outPath)) File.Delete(outPath);
        ZipFile.CreateFromDirectory(path, outPath);
    }
}
