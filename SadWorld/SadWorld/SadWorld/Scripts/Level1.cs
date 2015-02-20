using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SadWorld.Scripts
{
    static public class Level1
    {

        static Texture2D movementTutorial, box;
        static public List<Trigger> triggerList = new List<Trigger>();

        static bool isTutorialActive = false;
        static bool[] tutorialKeys = new bool[4]; //0: A, 
        static bool hasCompletedTutorial = false; //1: D, 
                                                  //2: E
                                                  //3: Space
        //trigger2 variables
        static bool hasTriggered2 = false;
        static bool reachedTop = false;
        static double timer = 0d;
        static bool trigger2_active = false;
        static Vector2 oldCamPosition;

        static public bool hasCamera = false;

        static public void LoadContent(ContentManager content)
        {
            movementTutorial = content.Load<Texture2D>("TextPopups/Level1/TutorialControls");
            box = content.Load<Texture2D>("box");
        }

        static public void Update(KeyboardState keys, ref SadWorld.Game1.SubGameState subGameState, GameTime gametime)
        {
            
            foreach(Trigger trig in triggerList)
                if (new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60).Intersects(trig.rect))
                switch(trig.triggerNumber)
                {
                    case 1:
                        {
                            if (!isTutorialActive)
                                isTutorialActive = true;
                            
                        }
                        break;
                    case 2:
                        {
                            if (!hasTriggered2)
                            {
                                hasTriggered2 = true;
                                subGameState = Game1.SubGameState.Cutscene;
                                trigger2_active = true;
                                hasCamera = true;
                                oldCamPosition = Camera.location;
                            }
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }

            if (!hasCompletedTutorial && isTutorialActive)
            {
                if (keys.IsKeyDown(Keys.A))
                    tutorialKeys[0] = true;
                if (keys.IsKeyDown(Keys.D))
                    tutorialKeys[1] = true;
                if (keys.IsKeyDown(Keys.E))
                    tutorialKeys[2] = true;
                if (keys.IsKeyDown(Keys.Space))
                    tutorialKeys[3] = true;

                if (tutorialKeys.All(check => check == true))
                    hasCompletedTutorial = true;
            }

            if (trigger2_active)
            {
                //true means done
                if (Trigger2())
                {
                    subGameState = Game1.SubGameState.None;
                    trigger2_active = false;
                    hasCamera = false;
                }

                timer += gametime.ElapsedGameTime.TotalMilliseconds;
                
            }

        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (!hasCompletedTutorial && isTutorialActive)
                spriteBatch.Draw(movementTutorial, new Vector2(1920 / 2 - movementTutorial.Width / 2, 50), Color.White);

            //foreach (Trigger trig in triggerList)
            //{
            //    spriteBatch.Draw(box, Camera.WorldToScreen(new Vector2(trig.rect.X, trig.rect.Y)), new Rectangle(0, 0, 40, 30), Color.White);
            //}
        }

        static bool Trigger2()
        {
            if (!reachedTop)
            {
                Camera.location -= new Vector2(0, 10);
                if (Vector2.Distance(Camera.location, oldCamPosition) > 900)
                    reachedTop = true;

                return false;
            }
            else
            {
                if (timer > 4000)
                    Camera.location += new Vector2(0, 30);

                if (Camera.location.Y > oldCamPosition.Y)
                    return true;
                return false;
            }
            
            
        }
    }
}
