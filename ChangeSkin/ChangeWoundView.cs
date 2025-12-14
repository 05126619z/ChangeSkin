using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ChangeSkin
{
    internal static class ChangeWoundView
    {
        internal static GameObject limbs;
        internal static void ToggleOn()
        {
            if (limbs == null)
            {
                limbs = WoundView.view.gameObject.transform.GetChild(1).gameObject;
            }
            Config.replaceWoundView = true;
            PreloadWoundViewTextures();
            PreloadWoundViewSprites();
            SaveOriginalSprites();
            ReplaceSprites();
        }
        internal static void ToggleOff()
        {
            Config.replaceWoundView = false;
            WoundView.view.StopAllCoroutines();
            ReturnTextures();
            TextureStorage.woundViewTextures.Clear();
            TextureStorage.woundViewSprites.Clear();
            TextureStorage.originalWoundViewSprites.Clear();
        }
        static string[] filenames =
        {
            "abdomenMiddle",
            "abdomenOutline",
            "armMiddle",
            "armOutline",
            "crusMiddle",
            "crusOutline",
            "disfiguredExpWound",
            "eyeGoneExpWound",
            "footMiddle",
            "footOutline",
            "forearmMiddle",
            "forearmOutline",
            "handMiddle",
            "handOutline",
            "headMiddle",
            "headoutline",
            "thighMiddle",
            "thighOutline",
            "thoraxMiddle",
            "thoraxOutline"
        };

        internal static void PreloadWoundViewTextures()
        {
            foreach (string filename in filenames)
            {
                try
                {
                    string path = string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"/{Config.skinName}/Textures/WoundView/{filename}.png"); // Hacky concat! Hah!
                    TextureStorage.woundViewTextures.Add(filename, Utils.LoadTexture(path));
                    TextureStorage.woundViewTextures[filename].name = filename;
                    TextureStorage.woundViewTextures[filename].filterMode = FilterMode.Point;
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }
            }
        }

        internal static void PreloadWoundViewSprites()
        {
            foreach (Texture2D texture in TextureStorage.woundViewTextures.Values)
            {
                try
                {
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        8,
                        0,
                        SpriteMeshType.Tight
                    );
                    sprite.name = texture.name;
                    TextureStorage.woundViewSprites.Add(texture.name, sprite);
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }
            }
        }
        internal static void SaveOriginalSprites()
        {
            foreach (Image image in limbs.GetComponentsInChildren<Image>())
            {
                if (!TextureStorage.originalWoundViewSprites.ContainsKey(image.sprite.name))
                {
                    TextureStorage.originalWoundViewSprites.Add(image.sprite.name, image.sprite);
                }
            }
        }
        internal static IEnumerator ReplaceSprites()
        {
            foreach (Sprite sprite in TextureStorage.woundViewSprites.Values)
            {
                foreach (Image image in limbs.GetComponentsInChildren<Image>())
                {
                    if (image.sprite == null)
                    {
                        continue;
                    }
                    if (image.sprite.name == sprite.name)
                    {
                        image.sprite = sprite;
                    }
                }
                yield return null;
            }
            yield break;
        }
        internal static void ReturnTextures()
        {
            foreach (Image image in limbs.GetComponentsInChildren<Image>())
            {
                if (TextureStorage.originalWoundViewSprites.TryGetValue(image.sprite.name, out Sprite originalSprite))
                {
                    image.sprite = originalSprite;
                }
            }
        }
    }
}
