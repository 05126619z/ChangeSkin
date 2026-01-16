using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChangeSkin
{
    // Yuck!
    internal static class ChangeFacialExpression
    {
        static List<Sprite> facialSprites = new List<Sprite>();
        static List<Sprite[]> facialSpritesLists = new List<Sprite[]>();

        internal static void SwapFacialExpression(FacialExpression facialExpression)
        {
            if (facialSprites.Count == 0 || facialSpritesLists.Count == 0)
            {
                facialSprites.Add(facialExpression.defaultHead);
                facialSprites.Add(facialExpression.defaultHeadMouth);
                facialSprites.Add(facialExpression.defaultHeadMouthHalf);
                facialSprites.Add(facialExpression.eyesGone);
                facialSprites.Add(facialExpression.eyesGoneHealed);
                facialSpritesLists.Add(facialExpression.disfiguredHead);
                facialSpritesLists.Add(facialExpression.disfiguredHeadHeal);
            }
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadBack",
                out facialExpression.defaultHead
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadBackMouth",
                out facialExpression.defaultHeadMouth
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadBackMouthMini",
                out facialExpression.defaultHeadMouthHalf
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentEyeGone",
                out facialExpression.eyesGone
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentEyeGoneHealed",
                out facialExpression.eyesGoneHealed
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured1",
                out facialExpression.disfiguredHead[0]
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured2",
                out facialExpression.disfiguredHead[1]
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured3",
                out facialExpression.disfiguredHead[2]
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured1Healed",
                out facialExpression.disfiguredHeadHeal[0]
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured2Healed",
                out facialExpression.disfiguredHeadHeal[1]
            );
            TextureStorage.bodySprites.TryGetValue(
                "experimentHeadDisfigured3Healed",
                out facialExpression.disfiguredHeadHeal[2]
            );
        }

        internal static void UnSwapFacialExpression(FacialExpression facialExpression)
        {
            facialExpression.defaultHead = facialSprites[0];
            facialExpression.defaultHeadMouth = facialSprites[1];
            facialExpression.defaultHeadMouthHalf = facialSprites[2];
            facialExpression.eyesGone = facialSprites[3];
            facialExpression.eyesGoneHealed = facialSprites[4];
            facialExpression.disfiguredHead = facialSpritesLists[0];
            facialExpression.disfiguredHeadHeal = facialSpritesLists[1];
        }
    }
}
