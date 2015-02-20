using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadWorld
{
    public class Powerup
    {
        public Vector2 position;
        public Vector2 levelNum;
        public Vector2 interfacePosition;
        public int powerNum;
        public string powerType;
        public int listNumber;
        public bool hasPower = false;
        public float cooldown;
        public float duration;
        public bool onCooldown = false;
        public float cooldownTimer;
        public string description;
        public Vector2 velocity;
        public bool isStuck = false;

        // POWERUP ORDER
        // AURA -- JUMP -- TILES -- SPEED -- COLOR

        public Powerup(Vector2 Position, Vector2 LevelNumber, int PowerNum, string PowerType, string Description, float Cooldown, float Length)
        {
            interfacePosition = Position;
            levelNum = LevelNumber;
            powerType = PowerType;
            powerNum = PowerNum - 1;
            duration = Length;
            cooldown = Cooldown;
            description = Description;
        }
    }
}
