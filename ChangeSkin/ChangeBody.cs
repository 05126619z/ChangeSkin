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
    /// summary
    /// Main ChangeSkin component which handles replacement and skin storage.
    public class ChangeBody : MonoBehaviour, IDisposable
    {
        Body body;
        TextureStorage textureStorage = new();
        ChangeFacialExpression changeFacialExpression = new();
        string skinName;
        string loadedName;
        string skinURL;
        bool loaded = false;
        public bool working = false;
        public bool isLocal = false;

        public void LoadSkinLocal(string skinName)
        {
            loaded = false;
            this.skinName = skinName;
            isLocal = true;
            SkinLoader.LoadSkin(skinName, filenames, isLocal, ref textureStorage.newBodySprites);
            loadedName = skinName;
            loaded = true;
        }

        public void LoadSkinURL(string url, string skinName)
        {
            loaded = false;
            string archiveName = SkinLoader.DownloadRemote(url);
            SkinLoader.UnpackRemote(skinName, archiveName);
            skinURL=url;
            this.skinName = skinName;
            isLocal = false;
            SkinLoader.LoadSkin(skinName, filenames, isLocal, ref textureStorage.newBodySprites);
            loadedName = skinName;
            loaded = true;
        }

        private void OnSkinLoad() { }

        // internal void LoadFromURL(string url)
        // {

        // }

        public void BeginReplacement()
        {
            if (!loaded)
                return;
            if (body == null)
                body = gameObject.GetComponent<Body>();
            if (textureStorage.newBodySprites.Count == 0)
            {
                if (isLocal)
                {
                    LoadSkinLocal(skinName);
                }
                else
                {
                    LoadSkinURL(skinURL, skinName);
                }
            }
            if (textureStorage.oldBodySprites.Count == 0)
                SaveOriginalSprites();
            changeFacialExpression.SwapFacialExpression(
                body.GetComponentInChildren<FacialExpression>(),
                textureStorage.newBodySprites
            );
            working = true;
        }

        private void Start()
        {
            body = gameObject.GetComponent<Body>();
        }

        private void Update()
        {
            if (loaded && working)
            {
                StartCoroutine(ReplaceSprites());
            }
        }

        public void StopReplacement()
        {
            working = false;
            StopAllCoroutines();
            ReturnSprites();
            changeFacialExpression.UnSwapFacialExpression(
                body.GetComponentInChildren<FacialExpression>()
            );
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
                if (spriteRenderer.sprite == null)
                {
                    continue;
                }
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
