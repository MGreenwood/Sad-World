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
    public static class PowerupInterface
    {
        public static List<Powerup> powerupList = new List<Powerup>();
        static Texture2D powerupTextures;
        public static Rectangle interfaceRect;
        public static bool isOpen = false;
        public static Vector2 position;
        public static int width = 67;
        public static int height = 67;
        // POWERUP ORDER
        // AURA -- JUMP -- TILES -- SPEED -- COLOR
        public static void LoadContent(ContentManager content, Viewport view)
        {
            Powerup power = new Powerup(new Vector2(0 * width, 0 * height), Vector2.Zero, 1, "bomb", "Causes a massive explosion\nof life, rooting nearby enemies\nin place and hurting them\nCooldown:15\nDuration:1", 0f, 1f);
            power.hasPower = true;
            powerupList.Add(power);
            //cd  duration

            power = new Powerup(new Vector2(1 * width, 0 * height), Vector2.Zero, 2, "tiles", "reveals hidden platforms\nCooldown:10\nDuration:5", 0f, 5f); // tiles
            power.hasPower = true;
            powerupList.Add(power);


            power = new Powerup(new Vector2(0 * width, 1 * height), Vector2.Zero, 3, "deflect", "Will convert enemy projectiles\nto color and deflect\nthem to the enemy\nCooldown:5\nDuration:10", 0f, 10f); //aura
            power.hasPower = true;
            powerupList.Add(power);
            
            
            power = new Powerup(new Vector2(1 * width, 1 * height), Vector2.Zero, 4, "speed", "increases the speed in\nwhich you move\nCooldown:10\nDuration:3", 0f, 3f); //speed
            power.hasPower = true;
            powerupList.Add(power);

            power = new Powerup(new Vector2(0 * width, 2 * height), Vector2.Zero, 5, "aura", "Enlarges your aura\nallowing you to\neffect a large area\nCooldown:5\nDuration:5", 0f, 10f);//large aura
            power.hasPower = true;
            powerupList.Add(power);

            //power = new Powerup(new Vector2(1, 0), Vector2.Zero, 2, "jump", "empowers you to jump\nmuch higher\nCooldown:1\nDuration:3", 1f, 3f); // jump
            //power.hasPower = true;
            //powerupList.Add(power);

            position = new Vector2(1920 - 200, 300);
            powerupTextures = content.Load<Texture2D>(@"Textures\Environment\powerupSheet");
            interfaceRect = new Rectangle((int)position.X,(int)position.Y, 160, 120);
        }

        public static void CheckClick(Point mousePos, Vector2 screenScale)
        {
            Vector2 modMouse = new Vector2(mousePos.X - 120, mousePos.Y - 30) * (1 + screenScale.Y);

            foreach (Powerup power in powerupList)
            {
                Rectangle powerRect = new Rectangle((int)(position.X + power.interfacePosition.X), (int)(position.Y + power.interfacePosition.Y), 67, 67);

                if (powerRect.Contains(new Point(Convert.ToInt32(modMouse.X), Convert.ToInt32(modMouse.Y))))
                {
                    SkillBar.AddPower(power);
                }
            }
        }

        public static string CheckHover(Point mousePos)
        {
            foreach(Powerup power in powerupList)
            {
                Rectangle powerRect = new Rectangle((int)(position.X + power.interfacePosition.X), (int)(position.Y + power.interfacePosition.Y), 67, 67);

                if (powerRect.Contains(mousePos))
                {
                    return power.description;                    
                }
            }

            return null;
        }
        
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Powerup power in powerupList)
            {
                if (power.hasPower)
                {
                    Color color = Color.White;
                    foreach(Powerup powerup in SkillBar.skillList)
                    if(powerup.powerNum == power.powerNum)
                        color.A = 10;
                    
                    spriteBatch.Draw(powerupTextures, position + power.interfacePosition, new Rectangle(width * power.powerNum, 0, 67, 67), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
        }
    }
}
