using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna;

namespace SadWorld
{
    public class Particle
    {
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public Color startingColor;

        Random rand = new Random();
        public float scale;
        public int startTime = 0;
        
        int speed;

        public Particle(Color startingColor)
        {
            scale = (float)rand.Next(5, 30) / 100;
            speed = rand.Next(11, 25);
            color = new Color(
                rand.Next(startingColor.R - 20, startingColor.R + 20),
                rand.Next(startingColor.G - 20, startingColor.G + 20),
                rand.Next(startingColor.B - 20, startingColor.B + 20));

        }
    }
}
