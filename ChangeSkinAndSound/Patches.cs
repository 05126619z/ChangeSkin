using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.Collections;
using UnityEngine.UIElements;
using System.Xml.Linq;
using static UnityEngine.UIElements.UIR.GradientSettingsAtlas;
using System.Threading.Tasks;

namespace ChangeSkin
{
    internal class Patches
    {

        static List<Texture2D> replacementTextures = new List<Texture2D>();
        static Dictionary<string, Sprite> replacementSprites = new Dictionary<string, Sprite>();
        static Dictionary<string, Sprite> originalSprites = new Dictionary<string, Sprite>();
        private static bool replaceTextures = false;
        public static string ToggleReplacement(string[] args)
        {
            
            if (args.Length == 0)
            {
                return "1 to toggle ON, 0 to toggle OFF, reload to reload";
            }
            if (args[0] == "1")
            {
                ToggleOn();
                return "Texture replacement ON";
            }
            else if (args[0] == "0")
            {
                ToggleOff();
                return "Texture replacement OFF";
            }
            else if (args[0] == "reload")
            {
                ToggleOff();
                ToggleOn();
                return "Textures reloaded";
            }
            return "1 to toggle ON, 0 to toggle OFF, reload to reload";
        }
        private static void ToggleOn()
        {
            PreloadTextures();
            PreloadSprites();
            replaceTextures = true;
            FixFacialExpression(PlayerCamera.main.body.GetComponentInChildren<FacialExpression>());
        }
        private static void ToggleOff()
        {
            replaceTextures = false;
            PlayerCamera.main.body.StopAllCoroutines();
            ReturnTextures();
            FixFacialExpression(PlayerCamera.main.body.GetComponentInChildren<FacialExpression>());
            replacementTextures.Clear();
            replacementSprites.Clear();
        }
        public delegate void orig_Body_Update(Body self);
        public static void Body_Update(orig_Body_Update orig, Body self)
        {
            orig(self);
            if (replaceTextures)
            {
                self.StartCoroutine(ReplaceTextures());
                //ReplaceSounds();
                //ReplaceEyes();
            }
        }
        /// <summary>
        /// Replaces textures of body
        /// </summary>
        /// <returns></returns>

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


        public static void PreloadTextures()
        {
            string pathToTextures = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Textures");
            foreach (string filename in filenames)
            {
                Texture2D texture = LoadTextureOnTexture2D(Path.Combine(pathToTextures, filename + ".png"));
                texture.name = filename;
                if (texture != null)
                {
                    replacementTextures.Add(texture);
                }
            }
        }

        public static void PreloadSprites()
        {
            foreach (Texture2D texture in replacementTextures)
            {
                texture.filterMode = FilterMode.Point;
                Sprite sprite;
                sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    8,
                    0,
                    SpriteMeshType.Tight
                );
                sprite.name = texture.name;
                //Plugin.logger.LogInfo(sprite.name);
                replacementSprites.Add(sprite.name, sprite);
                //replacementTextures.Remove(texture);
            }
        }

        static IEnumerator ReplaceTextures()
        {
            foreach (Sprite sprite in replacementSprites.Values)
            {
                foreach (SpriteRenderer spriteRenderer in PlayerCamera.main.body.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (spriteRenderer.sprite == null)
                    {
                        continue;
                    }
                    //Plugin.logger.LogInfo(spriteRenderer.name);
                    if (spriteRenderer.sprite.name == sprite.name)
                    {
                        if (!originalSprites.ContainsKey(spriteRenderer.sprite.name))
                        {
                            originalSprites.Add(spriteRenderer.sprite.name, spriteRenderer.sprite);
                            //Plugin.logger.LogInfo(spriteRenderer.name);
                            break;
                        }
                        spriteRenderer.sprite = sprite;
                    }
                }
                yield return null;
            }
            yield break;
        }

        static void ReturnTextures()
        {
            foreach (string spriteName in originalSprites.Keys)
            {
                foreach (SpriteRenderer spriteRenderer in PlayerCamera.main.body.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (spriteRenderer.sprite == null)
                    {
                        continue;
                    }
                    if (spriteRenderer.sprite.name == spriteName)
                    {
                        originalSprites.TryGetValue(spriteName, out Sprite oldSprite);
                        spriteRenderer.sprite = oldSprite;
                    }
                }
            }
        }

        static List<Sprite> facialSprites = new List<Sprite>();
        static List<Sprite[]> facialSpritesLists = new List<Sprite[]>();
        static void FixFacialExpression(FacialExpression facialExpression)
        {
            if (replaceTextures)
            {
                facialSprites.Add(facialExpression.defaultHead);
                facialSprites.Add(facialExpression.defaultHeadMouth);
                facialSprites.Add(facialExpression.defaultHeadMouthHalf);
                facialSprites.Add(facialExpression.eyesGone);
                facialSprites.Add(facialExpression.eyesGoneHealed);
                facialSpritesLists.Add(facialExpression.disfiguredHead);
                facialSpritesLists.Add(facialExpression.disfiguredHeadHeal);
                replacementSprites.TryGetValue("experimentHeadBack", out facialExpression.defaultHead);
                replacementSprites.TryGetValue("experimentHeadBackMouth", out facialExpression.defaultHeadMouth);
                replacementSprites.TryGetValue("experimentHeadBackMouthMini", out facialExpression.defaultHeadMouthHalf);
                replacementSprites.TryGetValue("experimentEyeGone", out facialExpression.eyesGone);
                replacementSprites.TryGetValue("experimentEyeGoneHealed", out facialExpression.eyesGoneHealed);
                replacementSprites.TryGetValue("experimentHeadDisfigured1", out facialExpression.disfiguredHead[0]);
                replacementSprites.TryGetValue("experimentHeadDisfigured2", out facialExpression.disfiguredHead[1]);
                replacementSprites.TryGetValue("experimentHeadDisfigured3", out facialExpression.disfiguredHead[2]);
                replacementSprites.TryGetValue("experimentHeadDisfigured1Healed", out facialExpression.disfiguredHeadHeal[0]);
                replacementSprites.TryGetValue("experimentHeadDisfigured2Healed", out facialExpression.disfiguredHeadHeal[1]);
                replacementSprites.TryGetValue("experimentHeadDisfigured3Healed", out facialExpression.disfiguredHeadHeal[2]);
            }
            if (!replaceTextures)
            {
                facialExpression.defaultHead = facialSprites[0];
                facialExpression.defaultHeadMouth = facialSprites[1];
                facialExpression.defaultHeadMouthHalf = facialSprites[2];
                facialExpression.eyesGone = facialSprites[3];
                facialExpression.eyesGoneHealed = facialSprites[4];
                facialExpression.disfiguredHead = facialSpritesLists[0];
                facialExpression.disfiguredHeadHeal = facialSpritesLists[1];
                facialSprites.Clear();
                facialSpritesLists.Clear();
            }
        }

        /// <summary>
        /// Loads textures from a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Texture from the path</returns>
        public static Texture2D LoadTextureOnTexture2D(string path)
        {
            try
            {

                byte[] bytes = File.ReadAllBytes(path);
                //FileStream file = File.OpenRead(path); // Useless cuz LoadImage autoresizes
                //Bitmap img = new Bitmap(file);
                //file.Dispose();
                Texture2D newTexture = new Texture2D(2, 2);
                newTexture.LoadImage(bytes);
                return newTexture;
            }
            catch (Exception e)
            {
                //Plugin.logger.LogDebug(e);
                return null;
            }
        }
    }
}
