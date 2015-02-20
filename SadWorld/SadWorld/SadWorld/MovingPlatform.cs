using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadWorld
{
    public class MovingPlatform
    {
        public bool isMovingRight;
        public bool isActive;
        public Vector2 position;

        public MovingPlatform(bool IsMovingRight, bool IsActive, Vector2 Position)
        {
            isMovingRight = IsMovingRight;
            isActive = IsActive;
            position = Position;
        }
    }
}
