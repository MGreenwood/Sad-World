using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SadWorld
{
    public class Infection
    {
        Vector2 position;
        double startTime;
        public bool isActive = false;
        public Rectangle rect;

        public void NewInfection(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalSeconds;

            Random random = new Random();
            position = new Vector2(Camera.location.X + 1920, random.Next(0, 630));
            isActive = true;
        }

        public void Update(GameTime gameTime)
        {
            if (position.X < -1280)
                isActive = false;

            if (isActive)
                position.X -= 5;

            rect = new Rectangle((int)position.X, (int)position.Y, 1280, 90);

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {

            spriteBatch.Draw(texture, Camera.WorldToScreen(position), Color.White);

        }
    }
}
