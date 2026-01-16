using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ModConfig
{
    public string cachedSkinName = "robot";
    public bool modEnabled = true; // unused

    public event Action<string> OnSkinNameChanged;
    public event Action<bool> OnModEnabledChanged;

    private string skinName;
    private bool enabled;

    public string CachedSkinName
    {
        get => skinName;
        set
        {
            if (skinName != value)
            {
                skinName = value;
                OnSkinNameChanged?.Invoke(value);
            }
        }
    }

    public bool ModEnabled
    {
        get => enabled;
        set
        {
            if (enabled != value)
            {
                enabled = value;
                OnModEnabledChanged?.Invoke(value);
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
