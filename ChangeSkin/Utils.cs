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
        static IEnumerator ReplaceSprite(GameObject gameobject, Sprite sprite)
        {
            try
            {
                SpriteRenderer spriteRenderer = gameobject.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    throw new Exception($"No SpriteRenderer in {gameobject.name}");
                }
                if (spriteRenderer.sprite.name == null)
                {
                    throw new Exception($"{gameobject.name} SpriteRenderer sprite contains no name");
                }
                if (sprite.name == null)
                {
                    throw new Exception($"Sprite name for {gameobject.name} cannot be empty");
                }
                spriteRenderer.sprite = sprite;
                //And just to be sure...
                spriteRenderer.sprite.name = sprite.name;
                yield break;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
                yield break;
            }
        }
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
                return newTexture;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
