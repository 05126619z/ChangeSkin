using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace ChangeSkin
{
    internal static class Utils
    {
        public static Texture2D LoadTexture(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes == null || bytes.Length == 0)
                {
                    throw new Exception($"Failed to read bytes from {path}");
                }
                Texture2D newTexture = new Texture2D(2, 2);
                newTexture.LoadImage(bytes);
                if (newTexture == null)
                {
                    throw new Exception($"Failed to load texture from {path}. Maybe it's not a texture?");
                }
                newTexture.filterMode = FilterMode.Point;
                return newTexture;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public static async Task<AudioClip> LoadAudio(string path)
        {
        AudioClip clip = null;
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
        uwr.SendWebRequest();

        try
        {
            while (!uwr.isDone) await Task.Delay(5);

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) Debug.Log($"{uwr.error}");
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(uwr);
            }
        }
        catch (Exception err)
        {
            Debug.Log($"{err.Message}, {err.StackTrace}");
        }
        }

        return clip;
        }
    }
}
