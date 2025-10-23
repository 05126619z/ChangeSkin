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
        internal static Dictionary<string, Texture2D> bodyTextures = new Dictionary<string, Texture2D>();
        internal static Dictionary<string, Sprite> bodySprites = new Dictionary<string, Sprite>();
        internal static Dictionary<string, Sprite> originalBodySprites = new Dictionary<string, Sprite>();
        internal static Dictionary<string, Texture2D> woundViewTextures = new Dictionary<string, Texture2D>();
        internal static Dictionary<string, Sprite> woundViewSprites = new Dictionary<string, Sprite>();
    }
}
