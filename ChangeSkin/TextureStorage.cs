using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChangeSkin
{
    internal static class TextureStorage
    {
        internal static Dictionary<string, Texture2D> bodyTextures = new();
        internal static Dictionary<string, Sprite> bodySprites = new();
        internal static Dictionary<string, Sprite> originalBodySprites = new();

        internal static Dictionary<string, Texture2D> woundViewTextures = new();
        internal static Dictionary<string, Sprite> woundViewSprites = new();
        internal static Dictionary<string, Sprite> originalWoundViewSprites = new();
    }
}
