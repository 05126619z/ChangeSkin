using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChangeSkin
{
    /// summary
    /// class that handles facial expression replacement
    internal class ChangeFacialExpression
    {
        List<Sprite> facialSprites = new List<Sprite>();
        List<Sprite[]> facialSpritesLists = new List<Sprite[]>();

        internal void SwapFacialExpression(
            FacialExpression facialExpression,
            Dictionary<string, Sprite> newFacialSprites
        )
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
            newFacialSprites.TryGetValue("experimentHeadBack", out facialExpression.defaultHead);
            newFacialSprites.TryGetValue(
                "experimentHeadBackMouth",
                out facialExpression.defaultHeadMouth
            );
            newFacialSprites.TryGetValue(
                "experimentHeadBackMouthMini",
                out facialExpression.defaultHeadMouthHalf
            );
            newFacialSprites.TryGetValue("experimentEyeGone", out facialExpression.eyesGone);
            newFacialSprites.TryGetValue(
                "experimentEyeGoneHealed",
                out facialExpression.eyesGoneHealed
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured1",
                out facialExpression.disfiguredHead[0]
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured2",
                out facialExpression.disfiguredHead[1]
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured3",
                out facialExpression.disfiguredHead[2]
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured1Healed",
                out facialExpression.disfiguredHeadHeal[0]
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured2Healed",
                out facialExpression.disfiguredHeadHeal[1]
            );
            newFacialSprites.TryGetValue(
                "experimentHeadDisfigured3Healed",
                out facialExpression.disfiguredHeadHeal[2]
            );
        }

        internal void UnSwapFacialExpression(FacialExpression facialExpression)
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
