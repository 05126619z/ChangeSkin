using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChangeSkin
{
    internal class TextureStorage
    {
        internal Dictionary<string, Sprite> newBodySprites = new();
        internal Dictionary<string, Sprite> oldBodySprites = new();
    }
}
