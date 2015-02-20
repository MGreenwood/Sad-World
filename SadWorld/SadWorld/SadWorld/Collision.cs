using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadWorld
{
    public class Collision
    {
        public void CollisionCheck(List<Rectangle> rectangles, bool hasAura, Viewport view, int rectCountNoMoving, Vector2 resolutionOffset, List<Vector2> collisionVectors)
        {
            Vector2 oldCamPosition = Camera.location; //store old position for use in determining how far to move the camera
            float sizeOffset =  view.Width/1920f;

            if (!Character.stuckToWall)
                Character.position += Character.velocity;
            else
                Character.position.Y += Character.velocity.Y;


            
            
            Rectangle playerRect = new Rectangle((int)Character.Position.X, (int)Character.Position.Y, 40, 60); // Rectangle for player

            if (rectangles != null)
                for(int rectNum=0;rectNum<rectangles.Count;rectNum++)
                {
                    
                    Rectangle rect = rectangles[rectNum];
                    int tileType = rect.Width;
                    

                    int tileSetWidth = Tile.TileSetTexture.Width / 40;
                    if (Vector2.Distance(new Vector2(rect.X,rect.Y), new Vector2(playerRect.X, playerRect.Y)) < 200 //only check tiles near player
                        && playerRect.Intersects(new Rectangle(rect.X, rect.Y, 40, 30)) //check for a general collision
                        && ((rect.Width != (tileSetWidth * 6 + 0)      // 
                        && rect.Width != (tileSetWidth * 6 + 1)        // hidden platforms
                        && rect.Width != (tileSetWidth * 6 + 2))       //
                        || (hasAura && PowerupInterface.powerupList[1].hasPower)))// if has aura and hidden tiles powerup
                    {
                        rect.Width = 40;
                        if (rect.Intersects(new Rectangle((int)collisionVectors[2].X, (int)collisionVectors[2].Y, 1,1)) ||
                            rect.Intersects(new Rectangle((int)collisionVectors[3].X, (int)collisionVectors[3].Y, 1,1)))
                            
                            //playerRect.X + 40 <= rect.X + 20                             //left
                            ////&& Character.velocity.X > 0
                            //&& playerRect.Y + 60 > rect.Y + 20
                            //&& playerRect.Y < rect.Y + 10)
                        {
                            Character.position.X = rect.X - (playerRect.Width - 1);
                            Character.velocity.X = 0;
                            Character.hitWall = true;
                            Character.hitLeft = true;
                        }
                        if (rect.Intersects(new Rectangle((int)collisionVectors[6].X, (int)collisionVectors[6].Y, 1, 1)) ||
                            rect.Intersects(new Rectangle((int)collisionVectors[7].X, (int)collisionVectors[7].Y, 1, 1)))
                            
                            //playerRect.Left >= rect.X + playerRect.Width - 15            //right
                            ////&& Character.velocity.X <= 0
                            //&& playerRect.Bottom > rect.Top + 20
                            //&& playerRect.Top < rect.Bottom - 20)
                        {
                            Character.position.X = rect.Right - 1;
                            Character.velocity.X = 0;
                            Character.hitWall = true;
                            Character.hitRight = true;
                        }

                        if (rect.Intersects(new Rectangle((int)collisionVectors[0].X, (int)collisionVectors[0].Y, 1, 1)) || 
                            rect.Intersects(new Rectangle((int)collisionVectors[1].X, (int)collisionVectors[1].Y, 1, 1)))
                            
                            //playerRect.Top >= rect.Bottom - 15                         //bottom
                            //&& Character.velocity.Y <= 0
                            //&& Character.position.X > rect.X - 35
                            //&& Character.position.X < rect.X + 35)
                        {
                            Character.position.Y = rect.Bottom + 1;
                            Character.velocity.Y = 0;
                        }
                        if (rect.Intersects(new Rectangle((int)collisionVectors[4].X, (int)collisionVectors[4].Y - 35, 1, 35)) || 
                            rect.Intersects(new Rectangle((int)collisionVectors[5].X, (int)collisionVectors[5].Y - 35, 1, 35)))
                            
                            //(playerRect.Bottom <= rect.Top + 20                           //top
                            //&& Character.velocity.Y >= 0
                            //&& Character.position.X > rect.X - 38
                            //&& Character.position.X < rect.X + 38)
                            //|| Character.position.Y > 3200)
                        {
                            if (rectNum >= rectCountNoMoving && Character.position.X > rect.X - 25 && Character.position.X < rect.X + 25)
                            {
                                Character.isOnMovingPlat = true;
                            }
                            Character.velocity.Y = 0;
                            Character.hitTop = true;
                            Character.hasDoubleJumped = false;

                            bool exists = false;
                            if (hasAura)
                            {
                                foreach (SpriteAnimation anim in Character.grassList)
                                {
                                    if (anim.Position == new Vector2(rect.X, rect.Y - 30))
                                    {
                                        exists = true;
                                        if (anim.CurrentAnimation == "die")
                                        {
                                            anim.CurrentAnimation = "grow";
                                        }
                                    }
                                }

                                if (!exists && (tileType == 1 || tileType == 2 || tileType == 3))
                                {
                                    SpriteAnimation grassAnimation = new SpriteAnimation(Character.grassTexture);
                                    grassAnimation.AddAnimation("grow", 40 * 0, 0, 40, 30, 4, .1f);
                                    grassAnimation.AddAnimation("die", 40 * 4, 0, 40, 30, 5, .1f);
                                    grassAnimation.IsAnimating = true;

                                    grassAnimation.CurrentAnimation = "grow";
                                    grassAnimation.Position = new Vector2(rect.X, rect.Y - 30);

                                    Character.grassList.Add(grassAnimation);
                                }
                            }
                            Character.position.Y = rect.Top - (playerRect.Height - 1);
                        }
                        
                            
                    }
                    else
                    {
                        if(hasAura)
                            for (int x = Character.grassList.Count - 1; x > -1; x--)
                            {
                                if (Character.grassList[x].Position == new Vector2(rect.X, rect.Y - 30))
                                {
                                    if (Character.grassList[x].CurrentFrameAnimation.CurrentFrame == 3 && Character.grassList[x].CurrentAnimation == "grow")
                                        Character.grassList[x].CurrentAnimation = "die";
                                     if (Character.grassList[x].CurrentFrameAnimation.CurrentFrame == 4)
                                    {
                                        Character.grassList.RemoveAt(x); 
                                    }
                                }
                            }
                    }
                }

            Character.position.X = MathHelper.Clamp(Character.position.X, 0, 1920 * 4 - 40);
            Camera.location = Character.position - new Vector2(1920 / 2, 1080 / 2);
            Camera.location.X = (int)MathHelper.Clamp(Camera.location.X, -resolutionOffset.X * sizeOffset, 5760 + (resolutionOffset.X * sizeOffset));
            Camera.location.Y = (int)MathHelper.Clamp(Camera.location.Y, 0, 2160);

            if (Vector2.Distance(Camera.location, oldCamPosition) < 100) //dont move for large corrections in camera position
                Character.parallaxOffset += (Camera.location - oldCamPosition) / 2; //generate offset based on Camera movement
        }
        
        public void EnemyCollisionCheck(List<Rectangle> rects, ref Rectangle enemyRect, ref Vector2 enemyVelocity)
        {
            foreach(Rectangle temp in rects)
            {
                Rectangle rect = new Rectangle(temp.X, temp.Y, 40,30);
                if(enemyRect.Intersects(rect))
                {
                    
                    if (enemyRect.X + 40 <= rect.X + 20  //left
                            && enemyVelocity.X > 0
                            && enemyRect.Y+60 > rect.Y + 30
                            && enemyRect.Y < rect.Y +10)
                    {
                        enemyRect.X = rect.X - 40;
                        enemyVelocity.X *= -1;
                    }
                    if (enemyRect.X >= rect.X + 25 //right
                        && enemyVelocity.X < 0
                        && enemyRect.Y + 60 > rect.Y + 30
                        && enemyRect.Y < rect.Y + 10)
                    {
                        enemyRect.X = rect.X + 40;
                        enemyVelocity.X *= -1;
                    }

                    if (enemyRect.Y >= rect.Bottom - 15 //bottom
                            && enemyRect.X > rect.X - 38
                            && enemyRect.X < rect.X + 38)
                    {
                        enemyRect.Y = rect.Y + 30;
                    }
                    if (enemyRect.Bottom <= rect.Top + 20 //top
                            && enemyVelocity.Y >= 0
                            && enemyRect.X > rect.X - 38
                            && enemyRect.X < rect.X + 38)
                    {
                        
                        enemyVelocity.Y = 0;
                        enemyRect.Y = rect.Y - 59;
                    }

                }
            }
            
        }
        public bool ProjectileCollisionCheck(List<Rectangle> rects, Rectangle projRect)
        {
            foreach (Rectangle rect in rects)
            {
                if(rect.Intersects(projRect))
                    return true;
            }
            return false;
        }
    }
}

        
    

