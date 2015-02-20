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
    static public class Character
    {
        static GamePadState oldGamepad;
        static public SpriteAnimation grassAnimation;
        static public SpriteAnimation playerAnim;
        static public Texture2D texture;
        static public Texture2D colorMap;
        static public Texture2D grassTexture;

        static Texture2D jumpPlatform;
        static int numJumpPlatforms = 0;
        public class JumpPlatform
        {
            public Vector2 position;
            public double timer;
            public bool isActive = false;

            public JumpPlatform(Vector2 Position, double Timer) //keep a nent!!!! manage it to see how many you have then loop through moving them back. 0 will always run out first!
            {
                position = Position;
                timer = Timer;
            }
        }
        static JumpPlatform[] jumpPlats = new JumpPlatform[5];

        static Texture2D particleTexture;

        //static double walkSoundTimer = 0d;

        static public bool isOnMovingPlat = false;
        static public Vector2 position;
        static public Vector2 velocity;
        static public Vector2 movementVector;
        static float speed = 10f;
        static float MAX_SPEED = 8f;
        static float SPRINT_SPEED = 12f;
        static float acceleration;
        static public float gravity = .55f;
        static public float WALL_GRAVITY = .3f;
        static public float JUMP_GRAVITY = -12f;
        static SoundEffect walk_sound;

        static public List<Projectile> playerProjectiles = new List<Projectile>();
        static MouseState oldMouse;

        static string direction = "East";
        static public Vector2 parallaxOffset;
        static public bool hasDoubleJumped = false;

        static public List<Vector2> collisionVectors = new List<Vector2>(); // 8 points of collision
        static Vector2 topLeft;
        static Vector2 topRight;
        static Vector2 rightTop;
        static Vector2 rightBottom;
        static Vector2 bottomRight;
        static Vector2 bottomLeft;
        static Vector2 leftBottom;
        static Vector2 leftTop;

        static public string[] powerList = new string[]{"None","Bomb","Tiles","Deflect","Speed","Aura"};
        //static public string powerUp = "none";
        public enum powerUp
        {
            None,
            Bomb,
            Tiles,
            Deflect,
            Speed,
            Aura
        };


        static public powerUp currentPowerup = powerUp.None;
        static public List<Powerup> powerUpList;
        static public List<Vector2> hiddenTiles =  new List<Vector2>();
        static public List<Powerup> usedPowerUps = new List<Powerup>();

        
        static public bool hitTop = true;
        static public bool hitWall = false;
        static bool oldHitwall = false;
        static double hitWallTimer = 0f;
        static public bool hitLeft = false;
        static public bool hitRight = false;
        static public bool sideCollision;
        static public bool stuckToWall = false;


        static float health;
        static public SpriteEffects effect;
        static KeyboardState oldKeys;

        static public List<Rectangle> collisionRect = null;
        static public List<Rectangle> portals = new List<Rectangle>(); // use the width of rectangle to say which portal it is.
        static public List<SpriteAnimation> grassList = new List<SpriteAnimation>();


        static Collision collision = new Collision();

        //static Viewport view;
       

        static public Vector2 DrawOffset { get; set; }
        static public float DrawDepth { get; set; }

        static public float Health
        {
            get { return health; }
            set { health = (int)MathHelper.Clamp(value, 0, 100); }
        }
        static public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        static public Vector2 Velocity
        {
            get { return velocity; }
        }

        static public void LoadContent(ContentManager Content)
        {
            health = 100;
            playerAnim = new SpriteAnimation(Content.Load<Texture2D>(@"Textures\Sprites\all_animations"));
            
            grassTexture = Content.Load<Texture2D>(@"Textures\Environment\grassAnim");

            playerAnim.AddAnimation("walkWest", 0, 60 * 0 , 40, 60, 4, .08f);
            playerAnim.AddAnimation("walkEast", 0, 60 * 1, 40, 60, 4, .08f);
            playerAnim.AddAnimation("idleWest", 0, 60 * 2 , 40, 60, 1, 0f);
            playerAnim.AddAnimation("idleEast", 40, 60 * 2, 40, 60, 1, 0f);
            playerAnim.AddAnimation("jumpWest", 40 * 4, 2 , 40, 60, 1, 0f);
            playerAnim.AddAnimation("jumpEast", 40 * 5, 2 , 40, 60, 1, 0f);

            playerAnim.CurrentAnimation = "idleEast";
            playerAnim.IsAnimating = true;
            playerAnim.Position = position;

            jumpPlatform = Content.Load<Texture2D>("Textures/UI/jumpPlat");

            walk_sound = Content.Load<SoundEffect>("SoundEffects/player/walksound");

            particleTexture = Content.Load<Texture2D>("Particle");

            topLeft = new Vector2(15, 0); //top left            
            collisionVectors.Add(topLeft);
            topRight = new Vector2(25, 0); //top right            
            collisionVectors.Add(topRight);
            rightTop = new Vector2(40, 15); //right top
            collisionVectors.Add(rightTop);
            rightBottom = new Vector2(40, 40); //right bottom
            collisionVectors.Add(rightBottom);
            bottomRight = new Vector2(25, 60); //bottom right
            collisionVectors.Add(bottomRight);
            bottomLeft = new Vector2(15, 60); //bottom left
            collisionVectors.Add(bottomLeft);
            leftBottom = new Vector2(0, 40); //left bottom
            collisionVectors.Add(leftBottom);
            leftTop = new Vector2(0, 15); //left top
            collisionVectors.Add(leftTop);
            
        }

        static public void Update(GameTime gameTime, GraphicsDevice graphics, List<Rectangle> rectangles, 
            List<Rectangle> movingPlatforms, bool hasAura, Vector2 levelNumber, Viewport view, Vector2 resolutionOffset, MouseState mouse, Vector2 mousePos)
        {

            health += .1f;
            /*
             * This small section of code 
             * makes the player stick to walls
             * making walljumpinbg controls a bit tightrer.
             * 
             * */
            oldHitwall = hitWall;

            if (hitWallTimer > 0d)
            {
                stuckToWall = true;
                hitWallTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (hitWallTimer > 250) //250 milliseconds
                {
                    stuckToWall = false;
                    hitWallTimer = 0d;
                }
            }
            
            /*
             * End Wall jumping code
             */


            collisionVectors[0] = position + topLeft;
            collisionVectors[1] = position + topRight;
            collisionVectors[2] = position + rightTop;
            collisionVectors[3] = position + rightBottom;
            collisionVectors[4] = position + bottomRight;
            collisionVectors[5] = position + bottomLeft;
            collisionVectors[6] = position + leftBottom;
            collisionVectors[7] = position + leftTop;

            acceleration = speed / 5;
            collisionRect = rectangles;
            
            string animation = "";
            view = graphics.Viewport;

            KeyboardState keys = Keyboard.GetState();

            List<int> removeList = new List<int>();

            
            
            PlayerIndex playerIndex = new PlayerIndex();
            Rectangle playerRect = new Rectangle((int)position.X, (int)position.Y, 40, 60);
            //foreach (Powerup power in powerUpList)
            //{
            //    Rectangle powRect = new Rectangle((int)power.position.X, (int)power.position.Y, 40, 30);

            //    if (!usedPowerUps.Any(pow => pow.powerType == power.powerType && pow.position == power.position) && powRect.Intersects(playerRect) && power.levelNum == levelNumber)
            //    {
            //        powerUp = power.powerType;
            //        usedPowerUps.Add(power);
            //        powerUpList.Remove(power);
            //        break;
            //    }
            //}

            if (mouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != oldMouse.LeftButton)
            {
                Projectile proj = new Projectile(new Vector2(position.X + 20, position.Y + 30), mousePos/*Camera.ScreenToWorld(new Vector2(mouse.X, mouse.Y))*/, particleTexture, 0);
                playerProjectiles.Add(proj);
                health -= 5;
            }

            //true means delete
            List<int> delNums= new List<int>();

            for(int x=playerProjectiles.Count() - 1; x >= 0; x--)
            {
                
                Rectangle projRect = new Rectangle((int)playerProjectiles[x].position.X, (int)playerProjectiles[x].position.Y, 25, 25);
                if (collision.ProjectileCollisionCheck(rectangles, projRect))
                    playerProjectiles[x].hasIntersected = true;
                

                
                if (playerProjectiles[x].Update(gameTime, 0f, particleTexture) )
                {
                    delNums.Add(x);
                }
            }

            foreach(int delNum in delNums)
            {
                playerProjectiles.RemoveAt(delNum);
            }

            if ((keys.IsKeyDown(Keys.D) || GamePad.GetState(playerIndex).ThumbSticks.Left.X > 0))            // Move Right
            {
                if (hitTop)
                {
                    animation = "walkEast";
                    velocity.X += acceleration;
                }
                else
                    velocity.X += acceleration / 1.25f;

                direction = "East";
                
            }
            else if ((keys.IsKeyDown(Keys.A) || GamePad.GetState(playerIndex).ThumbSticks.Left.X < 0))       // Move Left
            {
                if (hitTop)
                {
                    velocity.X -= acceleration;
                    animation = "walkWest";
                }
                else
                    velocity.X -= acceleration / 1.25f;

                direction = "West";
                
            }
            else if (keys.IsKeyUp(Keys.D) && keys.IsKeyUp(Keys.A))
            {
                float oldVel = velocity.X;

                if (velocity.X > 0)
                    velocity.X -= acceleration/2;
                else if (velocity.X < 0)
                    velocity.X += acceleration/2;

                if (oldVel > 0 && velocity.X < 0
                    || oldVel < 0 && velocity.X > 0)
                    velocity.X = 0;
                
                if(hitTop)
                playerAnim.CurrentAnimation = "idle" + direction; // playerAnim.CurrentAnimation.Substring(4);
            }
            

            // position.X = MathHelper.Clamp(position.X, 0, view.Width - 40);
            if(keys.IsKeyDown(Keys.LeftShift))
                velocity.X = MathHelper.Clamp(velocity.X, -SPRINT_SPEED, SPRINT_SPEED);
            else if (hitWall)
                velocity.X = MathHelper.Clamp(velocity.X, MAX_SPEED * -1.5f, MAX_SPEED * 1.5f);
            else
                velocity.X = MathHelper.Clamp(velocity.X, -MAX_SPEED, MAX_SPEED);

            velocity.Y = MathHelper.Clamp(velocity.Y, -25f, 25f); //VELOCITY CLAMP

            //if (velocity.X != 0 && hitTop)
            //{
            //    if (walkSoundTimer > 500d || walkSoundTimer == 0d)
            //    {
            //        walk_sound.Play(.5f, 0f, 0f);
            //        walkSoundTimer = 0d;
            //    }

            //    walkSoundTimer += gameTime.ElapsedGameTime.TotalMilliseconds; //sound effect for walking
            //}

            hitTop = false;

            int countRects = collisionRect.Count();

            
            

            //collisions
            if(movingPlatforms != null)
            foreach (Rectangle plat in movingPlatforms)
            {
                collisionRect.Add(plat);
            }

            collision.CollisionCheck(collisionRect, hasAura, view, countRects, resolutionOffset, collisionVectors);

            for (int x = collisionRect.Count(); x > countRects; x--)
            {
                collisionRect.RemoveAt(x - 1);
            }
            //end collisions


            
            playerAnim.Position = position;
            
            if (keys.IsKeyDown(Keys.W)) // health up
            {
                health += 1;
            }
            if (keys.IsKeyDown(Keys.S)) // health down
            {
                health -= 1;
            }

            if(health > 100)
                health = 100;


            if (!hitTop && !hitWall) //isnt on the ground
            {
                if (oldHitwall)
                {
                    hitWallTimer = 0d;
                    stuckToWall = false;
                }

                velocity.Y += .55f;
                playerAnim.CurrentAnimation = "jump" + direction;

                if (!hasDoubleJumped && (
                    (keys.IsKeyDown(Keys.Space) && keys.IsKeyDown(Keys.Space) != oldKeys.IsKeyDown(Keys.Space))
                    || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.A != oldGamepad.Buttons.A))
                {
                    velocity.Y = JUMP_GRAVITY;
                    hasDoubleJumped = true;

                    jumpPlats[numJumpPlatforms] = (new JumpPlatform(Character.position + new Vector2(-10, 50), 0d));
                    numJumpPlatforms++;
                }
            }
            else if (hitWall)
            {
                if (oldHitwall != hitWall)
                {
                    hitWallTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                if (velocity.Y < 0)
                    gravity = .55f;
                else 
                    gravity = WALL_GRAVITY;

                if ((keys.IsKeyDown(Keys.Space) && keys.IsKeyDown(Keys.Space) != oldKeys.IsKeyDown(Keys.Space)) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.A != oldGamepad.Buttons.A) //is on wall, can jump
                {
                    if (SkillBar.activeSkill.powerType == "jump")
                        gravity = JUMP_GRAVITY * 2;
                    else
                        gravity = JUMP_GRAVITY;

                    if (hitLeft && !hitTop)
                    {
                        velocity.X = -SPRINT_SPEED;
                           hitLeft = false;
                    }
                    else if(hitRight && !hitTop)
                    {
                        velocity.X = SPRINT_SPEED;
                        hitRight = false;
                    }

                    velocity.Y = gravity;
                    stuckToWall = false;
                    hitWallTimer = 0d;

                    playerAnim.CurrentAnimation = "jump" + direction;
                }
                else
                    velocity.Y += gravity;

                
                velocity.Y = MathHelper.Clamp(velocity.Y, -20, 25);
                hitRight = false;
                hitLeft = false;
                hitWall = false;
            }
            else                                  // is on ground
            {
                if ((keys.IsKeyDown(Keys.Space) && keys.IsKeyDown(Keys.Space) != oldKeys.IsKeyDown(Keys.Space)) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.A != oldGamepad.Buttons.A) //is on ground, can jump
                {
                    if (SkillBar.activeSkill.powerType == "jump")
                        gravity = -20f;
                    else
                        gravity = JUMP_GRAVITY;
                    
                    velocity.Y = gravity;
                    hitTop = false;
                    playerAnim.CurrentAnimation = "jump" + direction;
                }
            }

            if (position.Y > 3151)
                position.Y = 3151;

            if (velocity.X != 0 && !UserInterface.justShot)
                {
                    if (playerAnim.CurrentAnimation != animation)
                        playerAnim.CurrentAnimation = animation;
                }
           

            view = graphics.Viewport;
            //if (position.Y > view.Height) // if fall, move to top
            //    position.Y = 0;
            oldKeys = keys;
            if (jumpPlats[0] != null && jumpPlats[0].timer > 1000)
            {
                for (int x = 0; x < numJumpPlatforms; x++)
                {
                    jumpPlats[x] = jumpPlats[x + 1];
                }
                
                numJumpPlatforms--;
            }
            else
            {
                for (int x = 0; x < numJumpPlatforms; x++)
                {
                    jumpPlats[x].timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            

            foreach (SpriteAnimation anim in grassList)
            {
                if (anim.CurrentAnimation == "grow" && anim.CurrentFrameAnimation.CurrentFrame != 3)
                {
                    anim.Update(gameTime);
                }
                if (anim.CurrentAnimation == "die" && anim.CurrentFrameAnimation.CurrentFrame != 4)
                {
                    anim.Update(gameTime);
                }
            }

            oldGamepad = GamePad.GetState(playerIndex);
            oldMouse = mouse;

            playerAnim.Update(gameTime);


        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            playerAnim.Position = position;
            
            playerAnim.Draw(spriteBatch, 0, 0);

            foreach (Projectile proj in playerProjectiles)
            {
                proj.Draw(spriteBatch);
            }

            for(int x=0;x<numJumpPlatforms;x++)
            {
                spriteBatch.Draw(jumpPlatform, Camera.WorldToScreen(jumpPlats[x].position), Color.White);    
            }
        }

        static public void DrawTiles(SpriteBatch spriteBatch)
        {
            

            foreach (Rectangle rect in collisionRect)  // draw colored tiles near player
            {
                int tileSetWidth = Tile.TileSetTexture.Width / 40;
                //if ((rect.X > position.X - 200 && rect.X < position.X + 200) &&
                //    rect.Y > position.Y - 200 && rect.Y < position.Y + 200) // change hardcoded values
                if (((rect.Width != (tileSetWidth * 6 + 0) && rect.Width != (tileSetWidth * 6 + 1) && rect.Width != (tileSetWidth * 6 + 2)) || PowerupInterface.powerupList[1].hasPower))
                    spriteBatch.Draw(
                            colorMap,
                            Camera.WorldToScreen(new Vector2(
                                rect.X, rect.Y)),
                            Tile.GetSourceRectangle(rect.Width - 1),
                            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                // add overlay grass draw here.
                foreach (SpriteAnimation grass in grassList)
                {
                    grass.Draw(spriteBatch, 0, 0);
                }
            }
        }
        static public void DrawPowerups(SpriteBatch spriteBatch, Vector2 Level, Texture2D texture)
        {
            foreach (Powerup power in powerUpList)
            {
                if (!usedPowerUps.Any(pow => pow.powerType == power.powerType && pow.position == power.position) && power.levelNum == Level)
                {
                    spriteBatch.Draw(
                        texture,power.position,
                        new Rectangle(40 * (power.powerNum), 0, 40,30),
                        Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }
            
    }
}
