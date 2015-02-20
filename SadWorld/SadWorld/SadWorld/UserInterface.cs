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
    static public class UserInterface
    {
        static Texture2D healthBar;
        static Texture2D cursor;
        static Texture2D healthBarFill;

        static Texture2D projectileTexture;
        static float timer = 0; // keeps track of current game time in seconds
        static float startTimer = -1; // sets a start time when a projectile is fired (default to -1 to pass if statement on load)

        static public List<Projectile> projectiles = new List<Projectile>();
        //static Rectangle dummyRect = new Rectangle();
        static Viewport view;
        static SpriteFont portalFont;

        static public bool justShot = false; //timer for facing to shoot


        static Vector2 mousePos;
        

        static public void LoadContent(ContentManager content)
        {
            healthBar = content.Load<Texture2D>(@"Textures\UI\healthBar");
            cursor = content.Load<Texture2D>(@"Textures\UI\cursor");
            projectileTexture = content.Load<Texture2D>(@"Textures\projectile");
            healthBarFill = content.Load<Texture2D>(@"Textures\UI\healthBarFill");
            portalFont = content.Load<SpriteFont>("portalFont");
        }

        static public void Update(GameTime gameTime, GraphicsDevice graphics, MouseState mouse, int holeSize)
        {
            
            mouse = Mouse.GetState();
            mousePos = new Vector2(mouse.X, mouse.Y);
            
            view = graphics.Viewport;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;


            //if (mouse.LeftButton == ButtonState.Pressed && timer - startTimer > .5f)
            //{
            //    Projectile projectileNew = new Projectile(Character.position + new Vector2(20, 30), new Vector2(mouse.X, mouse.Y));
            //    projectiles.Add(projectileNew);
            //    projectileNew.LoadContent(projectileTexture);
            //    startTimer = timer;

                
            //}
            // make player look in the direction he is shooting for .1f seconds
            if (timer - startTimer < .1f)
            {
                string direction;
                if (mouse.X >= Character.position.X + 20)
                    direction = "walkEast";
                else
                    direction = "walkWest";
                Character.playerAnim.CurrentAnimation = direction;
                justShot = true;
            }
            else
                justShot = false;


            // update projectiles and remove those off the screen
            //if (projectiles.Count >= 0)
            //{
            //    foreach (Projectile projectile in projectiles)
            //    {
            //        projectile.Update(gameTime, holeSize);
            //    }
            //    for (int i = projectiles.Count - 1; i >= 0; i--)
            //    {
            //        dummyRect = new Rectangle((int)projectiles[i].position.X, (int)projectiles[i].position.Y, 1, 1);
            //        if (!dummyRect.Intersects(view.Bounds))
            //        {
            //            projectiles.RemoveAt(i);
            //        }
            //    }
            //}

        }

        static public void Draw(SpriteBatch spriteBatch)
        {

            
            // draw mouse
            //spriteBatch.Draw(cursor, new Vector2(mousePos.X - cursor.Width / 2, mousePos.Y - cursor.Height / 2), Color.White);
            spriteBatch.DrawString(portalFont, Character.currentPowerup.ToString(), new Vector2(1150, 20), Color.Black);
            

        }
        static public void DrawProjectiles(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

        }

        static public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthBarFill, new Vector2(0,  - (healthBarFill.Width * Character.Health / 100 )), Color.White);
        }

        
    }
    
}
