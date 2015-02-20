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
    static public class HoverText
    {
        public static void Draw(SpriteFont font, string text, SpriteBatch spriteBatch, Viewport view, Texture2D boxTexture)
        {
            spriteBatch.Draw(boxTexture, new Vector2(1920, 1080) - font.MeasureString(text) - new Vector2(55, 55), new Rectangle(0, 0, (int)font.MeasureString(text).X + 10, (int)font.MeasureString(text).Y + 10), Color.White);
            spriteBatch.DrawString(font, text, new Vector2(1920, 1080) - font.MeasureString(text) - new Vector2(50, 50), Color.White);
        }
    }
}
