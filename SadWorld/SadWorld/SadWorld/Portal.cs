using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadWorld
{
    public class Portal
    {
        public Vector2 position;
        public int type; //0 = normal   1 = boss
        public int levelNumber;

        public Portal(Vector2 Position, int Type, int LevelNumber)
        {
            type = Type;
            position = Position;
            levelNumber = LevelNumber;
        }

    }
}
