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
    static public class SkillBar
    {
        //public static Rectangle skillBarRect;
        //static Vector2 skillOneOffset = new Vector2(150, 75);
        //static Vector2 skillTwoOffset = new Vector2(90, 150);
        //static Vector2 skillThreeOffset = new Vector2(-20, 150);
        //static Vector2 skillFourOffset = new Vector2(-80, 75);

        public static Powerup activeSkill;
        public static bool isActive = true;
        public static bool isExpanding = false;
        public static bool isCollapsing = false;

        static int[] offsetMax = new int[4];
        static float[] offsetCurrent = new float[4];
        static bool[] offsetIsIncreasing = new bool[4];

        public static List<Powerup> skillList;

        static Texture2D powerupTexture;

        public static void LoadContent(ContentManager content, Viewport view)
        {
            skillList = new List<Powerup>();
            powerupTexture = content.Load<Texture2D>(@"Textures\Environment\powerupSheet");

            offsetMax[0] = 6;
            offsetMax[1] = 6;
            offsetMax[2] = 6;
            offsetMax[3] = 6;

            offsetCurrent[0] = 4f;
            offsetCurrent[1] = 1f;
            offsetCurrent[2] = -1f;
            offsetCurrent[3] = -4f;

            offsetIsIncreasing[0] = true;
            offsetIsIncreasing[1] = true;
            offsetIsIncreasing[2] = true;
            offsetIsIncreasing[3] = true;

            

        }

        public static void RemovePower(int indexNumber)
        {
            if(skillList.Count() >= indexNumber)
            skillList.RemoveAt(indexNumber);
        }

        public static void AddPower(Powerup power)
        {
            if (skillList != null && skillList.Count() < 4 && !skillList.Contains(power))
            {
                skillList.Add(power);
            }
        }

        public static void CheckClick(Point mousePos, Viewport view)
        {
           
            
            for(int i = skillList.Count - 1; i >= 0; i--)
            {

                Rectangle skillRect = new Rectangle((int)skillList[i].position.X, (int)skillList[i].position.Y, 67, 67);

                if (skillRect.Contains(mousePos))
                {
                    skillList[i].position = new Vector2(PowerupInterface.position.X + skillList[i].position.X * 67, PowerupInterface.position.Y + skillList[i].position.Y * 67);
                    RemovePower(i);
                }
            }
        }

        public static void DrawSkillBar(SpriteBatch spriteBatch, Viewport view)
        {
            for (int j = 0; j < 4; j++) //bouce
            {
                if (offsetIsIncreasing[j]) //increasing
                {
                    if (offsetCurrent[j] < offsetMax[j])
                    {
                        offsetCurrent[j] += .25f;
                    }
                    else
                    {
                        offsetIsIncreasing[j] = false;
                        offsetCurrent[j] -= .25f;
                    }
                }
                else //decreasing
                {
                    if (offsetCurrent[j] > -offsetMax[j])
                    {
                        offsetCurrent[j] -= .25f;
                    }
                    else
                    {
                        offsetIsIncreasing[j] = true;
                        offsetCurrent[j] += .25f;
                    }
                }
            }
            //spriteBatch.Draw(BarTexture, new Vector2(skillBarRect.X, 0), Color.White);
            if(isActive)
            {
                float rotation = ((Character.position.X / view.Width) * 2 - 1) * -1;
                rotation = MathHelper.Clamp(rotation, -.5f, .4f);
                Vector2 offset = Vector2.Zero;
                for(int x = 0; x < skillList.Count; x++)
                {
                    switch (x)
                    {
                        case 0:
                            offset = new Vector2(60, -60);
                            break;
                        case 1:
                            offset = new Vector2(40, 30);
                            break;
                        case 2:
                            offset = new Vector2(-30, 55);
                            break;
                        case 3:
                            offset = new Vector2(-90, 10);
                            break;
                    }
                    skillList[x].position = Rotate(rotation + x, -200, Character.position + offset);
                        spriteBatch.Draw(powerupTexture,
                            skillList[x].position + new Vector2(0, -offsetCurrent[x]),
                            new Rectangle(skillList[x].powerNum * 67, 0, 67, 67),
                            Color.White,
                            0f,
                            Vector2.Zero, 1f,
                            SpriteEffects.None, 1f);
                        
                   
                }
            }
            else if (isExpanding)
            {
                drawExpand(spriteBatch, view);
            }
            else if (isCollapsing)
            {
                drawCollapse(spriteBatch, view);
            }
            
        }

        private static Vector2 Rotate(float angle, float distance, Vector2 centre)
        {
            return new Vector2((float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle))) + centre;
        }

        public static void drawExpand(SpriteBatch spriteBatch, Viewport view)
        {
            float rotation = ((Character.position.X / view.Width) * 2 - 1) * -1;
            rotation = MathHelper.Clamp(rotation, -.5f, .4f);
            Vector2 offset = Vector2.Zero;
            for (int x = 0; x < skillList.Count(); x++)
            {
                
                    switch (x)
                    {
                        case 0:
                            offset = new Vector2(60, -60);
                            break;
                        case 1:
                            offset = new Vector2(40, 30);
                            break;
                        case 2:
                            offset = new Vector2(-30, 55);
                            break;
                        case 3:
                            offset = new Vector2(-70, 5);
                            break;
                    }
                    if (!skillList[x].isStuck)
                    {
                        skillList[x].velocity = (Rotate(rotation + x, -200, new Vector2(Character.position.X + offset.X, Character.position.Y + offset.Y))) - skillList[x].position;
                        skillList[x].velocity.Normalize();
                        skillList[x].position += skillList[x].velocity * 6;
                        spriteBatch.Draw(powerupTexture,
                            skillList[x].position,
                            new Rectangle(skillList[x].powerNum * 67, 0, 67, 67),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            Vector2.Distance(skillList[x].position, Character.position + new Vector2(20,30)) / 150 - .1f,
                            SpriteEffects.None,
                            1f);

                        if (Vector2.Distance(skillList[x].position, Rotate(rotation + x, -200, new Vector2(Character.position.X + offset.X, Character.position.Y + offset.Y))) < 5)
                        {
                            skillList[x].isStuck = true;
                        }
                    }
                    else
                    {
                        skillList[x].position = Rotate(rotation + x, -200, new Vector2(Character.position.X + offset.X, Character.position.Y + offset.Y));
                        spriteBatch.Draw(powerupTexture,
                            skillList[x].position,
                            new Rectangle(skillList[x].powerNum * 67, 0, 67, 67),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            Vector2.Distance(skillList[x].position, Character.position) / 150,
                            SpriteEffects.None,
                            1f);
                    }
                
                
            }
            if (skillList.All(e => e.isStuck == true))
            {
                isActive = true;
                isExpanding = false;
                foreach (Powerup power in skillList)
                    power.isStuck = false;
            }

        }

        public static void drawCollapse(SpriteBatch spriteBatch, Viewport view)
        {
            
            foreach (Powerup power in skillList)
            {
                if (!power.isStuck)
                {
                    power.velocity = (Character.position + new Vector2(20, 30)) - power.position;
                    power.velocity.Normalize();
                    power.position += power.velocity * 6;
                    spriteBatch.Draw(powerupTexture,
                        power.position,
                        new Rectangle(power.powerNum * 67, 0, 67, 67),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.Distance(power.position, Character.position + new Vector2(20, 30)) / 150,
                        SpriteEffects.None,
                        1f);

                    if (Vector2.Distance(Character.position + new Vector2(20, 30), power.position) < 10)
                    {
                        power.isStuck = true;
                    }
                }
                

            }
            if (skillList.All(e => e.isStuck == true))
            {
                isActive = false;
                isCollapsing = false;
                foreach (Powerup power in skillList)
                    power.isStuck = false;
            }
            
        }
    }
}
