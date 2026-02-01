using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            Texture2D newTexture = new Texture2D(2, 2);
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                newTexture.LoadImage(bytes);
                newTexture.filterMode = FilterMode.Point;
            }
            catch (Exception e)
            {
                newTexture = Resources.Load<Texture2D>("WarningSign");
                throw(e);
            }
            return newTexture;
        }

        public static Sprite LoadSprite(string path)
        {
            Sprite outSprite = new();
            try
            {
                Texture2D tex = LoadTexture(path);
                outSprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    8,
                    0,
                    SpriteMeshType.Tight
                );
            }
            catch (Exception e)
            {
                outSprite = Resources.Load<Sprite>("WarningSign");
                throw(e);
            }
            return outSprite;
        }
        

        // public static async Task<AudioClip> LoadAudio(string path)
        // {
        // AudioClip clip = null;
        // using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        // {
        // uwr.SendWebRequest();

        // try
        // {
        //     while (!uwr.isDone) await Task.Delay(5);

        //     if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) Debug.Log($"{uwr.error}");
        //     else
        //     {
        //         clip = DownloadHandlerAudioClip.GetContent(uwr);
        //     }
        // }
        // catch (Exception err)
        // {
        //     Debug.Log($"{err.Message}, {err.StackTrace}");
        // }
        // }

        // return clip;
        // }
    }
}
