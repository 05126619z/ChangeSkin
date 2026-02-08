using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[Serializable]
public class ModConfig
{
    public string LastSelectedSkin;
    public string LastURL;
    public bool SkinUploading = true;
    public bool SkinDownloading = true;

    public void Save(string filePath)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        File.WriteAllText(filePath, json);
    }

    public static ModConfig Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            ModConfig config = new();
            config.Save(filePath);
            return config;
        }

        string json = File.ReadAllText(filePath);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<ModConfig>(json);
    }
}
