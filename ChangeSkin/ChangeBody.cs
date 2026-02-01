using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChangeSkin
{
    public class ChangeBody : MonoBehaviour, IDisposable
    {
        Body body;
        TextureStorage textureStorage = new();
        ChangeFacialExpression changeFacialExpression = new();
        string skinName;
        public bool working = false;

        // internal void LoadFromServer(string skinName)
        // {
        //     // this.skinName = skinName;
        // }

        public void LoadSkin(string skinName)
        {
            this.skinName = skinName;
            try
            {
                textureStorage.newBodySprites.Clear();
                SkinLoader.LoadFromFolder(skinName, filenames, ref textureStorage.newBodySprites);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        // internal void LoadFromURL(string url)
        // {

        // }

        public void BeginReplacement()
        {
            if (body == null)
                body = gameObject.GetComponent<Body>();
            if (skinName == null)
            {
                Plugin.Instance.Logger.LogInfo("No skin for body selected");
                return;
            }
            ;
            if (textureStorage.newBodySprites.Count == 0)
                LoadSkin(skinName);
            if (textureStorage.oldBodySprites.Count == 0)
                SaveOriginalSprites();
            changeFacialExpression.SwapFacialExpression(
                body.GetComponentInChildren<FacialExpression>(),
                textureStorage.newBodySprites
            );
            working = true;
        }

        private void Start() { }

        private void Update()
        {
            if (working)
            {
                StartCoroutine(ReplaceSprites());
            }
        }

        public void StopReplacement()
        {
            StopAllCoroutines();
            ReturnSprites();
            changeFacialExpression.UnSwapFacialExpression(
                body.GetComponentInChildren<FacialExpression>()
            );
            working = false;
            StopCoroutine(ReplaceSprites());
        }

        public void Dispose()
        {
            StopReplacement();
        }

        internal void SaveOriginalSprites()
        {
            foreach (
                SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>()
            )
            {
                if (!textureStorage.oldBodySprites.ContainsKey(spriteRenderer.sprite.name))
                {
                    textureStorage.oldBodySprites.Add(
                        spriteRenderer.sprite.name,
                        spriteRenderer.sprite
                    );
                }
            }
        }

        internal IEnumerator ReplaceSprites()
        {
            foreach (Sprite sprite in textureStorage.newBodySprites.Values)
            {
                foreach (
                    SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>()
                )
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

        internal void ReturnSprites()
        {
            foreach (
                SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>()
            )
            {
                if (
                    textureStorage.oldBodySprites.TryGetValue(
                        spriteRenderer.sprite.name,
                        out Sprite originalSprite
                    )
                )
                {
                    spriteRenderer.sprite = originalSprite;
                }
            }
        }

        static readonly string[] filenames =
        {
            "Body/experimentTail.png",
            "Body/experimentFoot.png",
            "Body/experimentUpTorso.png",
            "Body/experimentUpArm.png",
            "Body/experimentThigh.png",
            "Body/experimentDownTorso.png",
            "Body/experimentDownArm.png",
            "Body/experimentCrus.png",
            "Body/experimentEyeGoneHealed.png",
            "Body/experimentEyeGone.png",
            "Body/experimentEyeClosed.png",
            "Body/experimentEyeScaredBack.png",
            "Body/experimentEyeScared.png",
            "Body/experimentEyeSadBack.png",
            "Body/experimentEyeSad.png",
            "Body/experimentHead.png",
            "Body/experimentEyePanic.png",
            "Body/experimentEyeOpen.png",
            "Body/experimentEyeLookBack.png",
            "Body/experimentEyeHalfClosedBack.png",
            "Body/experimentEyeHalfClosed.png",
            "Body/experimentHeadDisfigured3Healed.png",
            "Body/experimentHeadDisfigured3.png",
            "Body/experimentHeadDisfigured2Healed.png",
            "Body/experimentHeadDisfigured2.png",
            "Body/experimentHeadDisfigured1Healed.png",
            "Body/experimentHeadDisfigured1.png",
            "Body/experimentHeadBackMouth.png",
            "Body/experimentHeadBackMouthMini.png",
            "Body/experimentHeadBack.png",
            "Body/experimentHandB.png",
            "Body/experimentHandF.png",
            "Body/experimentNosebleed.png",
        };
    }
}
