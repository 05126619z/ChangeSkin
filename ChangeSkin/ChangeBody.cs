using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChangeSkin
{
    internal class ChangeBody
    {
        internal static void ToggleOn()
        {
            Config.replaceBody = true;
            PreloadBodyTextures();
            PreloadBodySprites();
            ChangeFacialExpression.SwapFacialExpression(PlayerCamera.main.body.GetComponentInChildren<FacialExpression>());
        }
        internal static void ToggleOff()
        {
            Config.replaceBody = false;
            PlayerCamera.main.body.StopAllCoroutines();
            ReturnTextures();
            ChangeFacialExpression.UnSwapFacialExpression(PlayerCamera.main.body.GetComponentInChildren<FacialExpression>());
            TextureStorage.bodyTextures.Clear();
            TextureStorage.bodySprites.Clear();
        }
        static string[] filenames =
        {
            "experimentTail",
            "experimentFoot",
            "experimentUpTorso",
            "experimentUpArm",
            "experimentThigh",
            "experimentDownTorso",
            "experimentDownArm",
            "experimentCrus",
            "experimentEyeGoneHealed",
            "experimentEyeGone",
            "experimentEyeClosed",
            "experimentEyeScaredBack",
            "experimentEyeScared",
            "experimentEyeSadBack",
            "experimentEyeSad",
            "experimentHead",
            "experimentEyePanic",
            "experimentEyeOpen",
            "experimentEyeLookBack",
            "experimentEyeHalfClosedBack",
            "experimentEyeHalfClosed",
            "experimentHeadDisfigured3Healed",
            "experimentHeadDisfigured3",
            "experimentHeadDisfigured2Healed",
            "experimentHeadDisfigured2",
            "experimentHeadDisfigured1Healed",
            "experimentHeadDisfigured1",
            "experimentHeadBackMouth",
            "experimentHeadBackMouthMini",
            "experimentHeadBack",
            "experimentHandB",
            "experimentHandF"
        };

        internal static void PreloadBodyTextures()
        {
            foreach (string filename in filenames)
            {
                try
                {
                    string path = string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"/Textures/Body/{filename}.png"); // Hacky concat! Hah!
                    TextureStorage.bodyTextures.Add(filename, Utils.LoadTexture(path));
                    TextureStorage.bodyTextures[filename].name = filename;
                    TextureStorage.bodyTextures[filename].filterMode = FilterMode.Point;
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }
            }
        }

        internal static void PreloadBodySprites()
        {
            foreach (Texture2D texture in TextureStorage.bodyTextures.Values)
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
                    TextureStorage.bodySprites.Add(texture.name, sprite);
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }
            }
        }
        internal static void SaveOriginalSprites()
        {
            foreach (SpriteRenderer spriteRenderer in PlayerCamera.main.body.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (!TextureStorage.originalBodySprites.ContainsKey(spriteRenderer.sprite.name))
                {
                    TextureStorage.originalBodySprites.Add(spriteRenderer.sprite.name, spriteRenderer.sprite);
                }
            }
        }
        internal static IEnumerator ReplaceTextures()
        {
            if (TextureStorage.originalBodySprites.Count == 0) 
            {
                SaveOriginalSprites();
            }
            foreach (Sprite sprite in TextureStorage.bodySprites.Values)
            {
                foreach (SpriteRenderer spriteRenderer in PlayerCamera.main.body.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (spriteRenderer.sprite == null)
                    {
                        continue;
                    }
                    if (spriteRenderer.sprite.name == sprite.name)
                    {
                        spriteRenderer.sprite = sprite;
                    }
                }
                yield return null;
            }
            yield break;
        }
        internal static void ReturnTextures()
        {
            foreach (SpriteRenderer spriteRenderer in PlayerCamera.main.body.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (TextureStorage.originalBodySprites.TryGetValue(spriteRenderer.sprite.name, out Sprite originalSprite))
                {
                    spriteRenderer.sprite = originalSprite;
                }
            }
        }
    }
}
