using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ModConfig
{
    private string selectedSkin = "robot";

    public string SelectedSkin
    {
        get => selectedSkin;
        set
        {
            if (selectedSkin != value)
            {
                selectedSkin = value;
            }
        }
    }

    public void Save(string filePath)
    {
        string json = JsonUtility.ToJson(this, true);
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
        return JsonUtility.FromJson<ModConfig>(json);
    }
}
