using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;

namespace ChangeSkin
{
    internal class TextureStorage
    {
        internal Dictionary<string, Sprite> newBodySprites = [];
        internal static Dictionary<string, Sprite> ogSprites = [];

        
        internal static void SaveOGSprite(string filename)
        {
            Sprite sprite = Utils.LoadSprite(Paths.PluginPath + $"/ChangeSkin/resources/og/{filename}");
            ogSprites.Add(sprite.name, sprite);
        }

        internal static void SaveOGSprites()
        {
            foreach (string filename in TextureStorage.ogBodySpriteFilenames)
            {
                TextureStorage.SaveOGSprite(filename);
            }
        }

        internal static readonly string[] ogBodySpriteFilenames =
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
