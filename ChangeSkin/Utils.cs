using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    }
}
