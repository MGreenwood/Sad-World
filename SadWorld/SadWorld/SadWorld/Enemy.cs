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
    public class Enemy
    {
        public Vector2 position;
        public Vector2 startPosition;
        public Vector2 velocity;// =  new Vector2(-3,0);
        public int health = 100;
        public Texture2D texture;
        public Texture2D projectileTexture;
        public Texture2D particleTexture;
        public Texture2D particleTextureBad;
        Texture2D particleTextureGood;
        public int enemyType;
        Collision collision;
        SpriteAnimation animation;
        public List<Projectile> projectiles = new List<Projectile>();
        Rectangle dummyRect = new Rectangle();
        public int levelNumber;
        public int currentLevel;
        Rectangle playerRect;
        Rectangle enemyRect;
        public bool enemyHasAura = false;
        KeyboardState oldks;
        KeyboardState ks;
        int dialogueNum = 0;
        public List<string> dialogue = new List<string>();

        

        

        bool enemyDead = false;
        SpriteFont portalFont;

        float startTimer = -1;
        float timer = 0;
        static public bool justShot = false; //timer for facing to shoot
        

        public Enemy(ContentManager Content, Texture2D ParticleTexture, int type)
        {
            portalFont = Content.Load<SpriteFont>("portalFont");
            particleTextureBad = ParticleTexture;

            

            if (type == 0)
            {
                texture = Content.Load<Texture2D>(@"Textures\Sprites\all_animations");
                animation = new SpriteAnimation(texture);
                animation.AddAnimation("walkWest", 0, 60 * 0, 40, 60, 4, .08f);
                animation.AddAnimation("walkEast", 0, 60 * 1, 40, 60, 4, .08f);
                animation.AddAnimation("idleWest", 0, 60 * 2, 40, 60, 1, 0f);
                animation.AddAnimation("idleEast", 40, 60 * 2, 40, 60, 1, 0f);

                
                

                animation.CurrentAnimation = "idleWest";
            }
            else if (type == 3)
            {
                texture = Content.Load<Texture2D>(@"Textures\Sprites\oldblob");
                animation = new SpriteAnimation(texture);
                animation.AddAnimation("idle", 0, 60 * 0, 60, 60, 1, 0f);
                animation.AddAnimation("talk", 60, 60 * 0, 60, 60, 3, .08f);
                animation.AddAnimation("die", 0, 60 * 1, 60, 60, 6, .08f);

                animation.CurrentAnimation = "idle";
            }

            animation.IsAnimating = true;
        }
        public Enemy(ContentManager Content, Texture2D ParticleTextureGood, Texture2D ParticleTextureBad, int type, Vector2 StartPosInput, int level)
        {
            levelNumber = level;
            position = StartPosInput;
            particleTexture = ParticleTextureBad;
            particleTextureBad = ParticleTextureBad;
            particleTextureGood = ParticleTextureGood;
            portalFont = Content.Load<SpriteFont>("portalFont");
            projectileTexture = Content.Load<Texture2D>(@"Textures\projectile");

            texture = Content.Load<Texture2D>(@"Textures\Sprites\all_animations");
            animation = new SpriteAnimation(texture);
            animation.AddAnimation("walkWest", 0, 60 * 0, 40, 60, 4, .08f);
            animation.AddAnimation("walkEast", 0, 60 * 1, 40, 60, 4, .08f);
            animation.AddAnimation("idleWest", 0, 60 * 2, 40, 60, 1, 0f);
            animation.AddAnimation("idleEast", 40, 60 * 2, 40, 60, 1, 0f);

            animation.CurrentAnimation = "idleWest";
            animation.IsAnimating = true;
            
        }

        public void Update(GameTime gameTime, Viewport view, Vector2 levelNum, ContentManager content, float holeSize, ref bool hasAura)
        {
            oldks = ks;
            ks = Keyboard.GetState();
            if(animation.CurrentAnimation != "die" || !enemyDead)
            animation.Update(gameTime);
            animation.Position = position;

            

            if (health > 0 && enemyType < 3)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;


                if (timer - startTimer > .5f)
                {
                    Projectile projectileNew = new Projectile(position + new Vector2(20, 30), new Vector2(Character.position.X + 20, Character.position.Y + 30), particleTextureBad, 1);
                    projectiles.Add(projectileNew);
                    projectileNew.LoadContent(projectileTexture, content);
                    startTimer = timer;
                }
                List<int> delList = new List<int>();
                // update projectiles and remove those off the screen
                if (projectiles.Count >= 0)
                {
                    for (int x = projectiles.Count() - 1; x > -1; x--)
                    {
                        if (Vector2.Distance(Character.position, projectiles[x].position) < 1500)
                        {
                            projectiles[x].Update(gameTime, holeSize, particleTextureGood);
                        }
                        else
                        {
                            delList.Add(x); // mark for deletion
                        }
                        
                    }
                    for (int j = 0; j <= delList.Count() - 1; j++)
                    {
                        projectiles.RemoveAt(delList[j]); //remove projectiles marked for deleteion
                    }
                    delList.Clear();
                    for (int i = projectiles.Count - 1; i >= 0; i--)
                    {
                        Projectile proj = projectiles[i];
                        if (new Rectangle((int)proj.position.X, (int)proj.position.Y, 50,50).Intersects(new Rectangle((int)position.X, (int)position.Y, 40,60)) && proj.hasDeflected && !proj.hasIntersected)
                        {
                            proj.hasIntersected = true;
                            health -= 10;
                        }
                        dummyRect = new Rectangle((int)projectiles[i].position.X, (int)projectiles[i].position.Y, 1, 1);
                        //if (!dummyRect.Intersects(view.Bounds) && levelNumber == currentLevel)
                        //{
                        //    projectiles.RemoveAt(i);
                        //}
                    }
                }
            }
            else
            {
                animation.Position = position;
                projectiles.Clear();
                if (Character.position.X <= position.X && enemyType < 3)
                {
                    animation.CurrentAnimation = "idleWest";
                }
                else if (Character.position.X > position.X && enemyType < 3)
                    animation.CurrentAnimation = "idleEast";

                playerRect = new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60);
                enemyRect = new Rectangle((int)position.X - 40, (int)position.Y, 120, 60);
                if(health > 0)
                {
                    switch(enemyType)
                    {
                        case 1:
                            {
                                break;
                            }
                        case 2:
                            {
                                break;
                            }
                        case 3: //gives player aura
                            {
                                if (playerRect.Intersects(enemyRect) && ks.IsKeyDown(Keys.E) && oldks.IsKeyDown(Keys.E) != ks.IsKeyDown(Keys.E))
                                {
                                    if (dialogueNum == dialogue.Count() - 1)
                                    {
                                        animation.CurrentAnimation = "die";
                                        dialogueNum += 1;
                                        hasAura = true;
                                        enemyHasAura = false;
                                    }
                                    else
                                        dialogueNum += 1;
                                }
                                
                                
                                break;
                            }
                        case 4:
                            {
                                break;
                            }
                        case 5:
                            {
                                break;
                            }
                        case 6:
                            {
                                break;
                            }

                    }
                }
            }
            
        }

        public void Patrol(List<Rectangle> rects)
        {
            int oldXvelocity = (int)velocity.X;

            collision = new Collision();

            Rectangle enemyRect = new Rectangle((int)position.X, (int)position.Y, 40, 60);
            velocity.Y += .55f;

            if (velocity.X > 0)
            {
                position.X += velocity.X;
                if ((position.X - startPosition.X) > 600f)
                {
                    velocity *= -1;
                    position.X -= velocity.X * 2;
                }
            }
            else
            {
                position.X -= velocity.X;
                if ((startPosition.X - position.X) > 600f)
                {
                    velocity *= -1;
                    position.X += velocity.X * 2;
                }
            }


                collision.EnemyCollisionCheck(rects, ref enemyRect, ref velocity);
            
            if(position != new Vector2(enemyRect.X, enemyRect.Y))
            {
                position = new Vector2(enemyRect.X, enemyRect.Y);
                //velocity.X *= -1;
            }
            
            
            
            

            if (velocity.X > 0 && animation.CurrentAnimation != "walkEast")
                animation.CurrentAnimation = "walkEast";
            if (velocity.X < 0 && animation.CurrentAnimation != "walkWest")
                animation.CurrentAnimation = "walkWest";
            if (velocity.X == 0)
            {
                if (oldXvelocity > 0)
                    animation.CurrentAnimation = "idleEast";
                else
                    animation.CurrentAnimation = "idleWest";
            }
            Random rand = new Random();
            for(int x = projectiles.Count() - 1; x > -1; x--) 
            {
                foreach (Rectangle rect in rects)
                {
                    if(new Rectangle((int)projectiles[x].position.X, (int)projectiles[x].position.Y, 25,25).Intersects(rect))
                    {
                        projectiles[x].timeToExplode = true;
                        projectiles[x].explosionEmitter = new ParticleEmitter(new Vector2(rand.Next(-5, 5), 0), particleTextureBad, Color.Purple, Color.Green, new Vector2(0, rand.Next(1, 10)), false);
                        projectiles[x].hasIntersected = true;
                        break;
                    }
                    
                }
                  if (projectiles[x].deleteMe == true)
                    projectiles.RemoveAt(x);
            }
            
            position += velocity;
            animation.Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentLevel == levelNumber && animation.IsAnimating)
            {
                

                if (animation.CurrentAnimation != "die")
                {
                    animation.Draw(spriteBatch, 0, 0);
                }
                else
                {
                    if (animation.CurrentFrameAnimation.CurrentFrame != 5)
                    {
                        animation.Draw(spriteBatch, 0, 0);
                    }
                    else
                    {
                        animation.Draw(spriteBatch, 0, 0);
                        enemyDead = true;
                        //animation.IsAnimating = false;
                    }
                }

                foreach (Projectile projectile in projectiles)
                    projectile.Draw(spriteBatch);

                if (enemyType == 3)
                {
                    if (playerRect.Intersects(enemyRect) && dialogueNum < dialogue.Count())
                    {
                        if(animation.CurrentAnimation != "talk")
                        animation.CurrentAnimation = "talk";
                        spriteBatch.DrawString(portalFont, dialogue[dialogueNum], Camera.WorldToScreen(position - new Vector2(40, 100)), Color.White);
                    }
                    else
                    {
                        if (animation.CurrentAnimation != "die")
                            animation.CurrentAnimation = "idle";
                    }
                }
                
            }
        }
    }
}
