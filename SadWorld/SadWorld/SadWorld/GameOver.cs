using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SadWorld
{
    public class GameOver
    {
        public  float alpha = 0;
        public bool goingUp = true;
        Texture2D fadeTexture;
        Color color;
        public bool fadeout = false;

        public GameOver(ContentManager content)
        {
            fadeTexture = content.Load<Texture2D>(@"Textures\UI\Fade");
        }

        public bool Update(GameTime gameTime)
        {
            if (goingUp && alpha != 100)
            {
                alpha += 1f;
            }
            else
            {
                if (alpha == 1 && fadeout)
                {
                    fadeout = false;
                }
                alpha -= 1f;
                if (alpha == 0)
                {
                    return true;
                }
            }
            if (alpha == 100)
            {
                goingUp = false;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 scale)
        {
            color = new Color(255, 255, 255, alpha/100);
            spriteBatch.Draw(fadeTexture, new Vector2(0,0), new Rectangle(0,0,1920,1080), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
