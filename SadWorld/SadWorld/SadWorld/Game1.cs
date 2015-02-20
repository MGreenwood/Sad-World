using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;



namespace SadWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        //array of Vector2 to be used to draw lingering aura
        
        /* //screen crap
         float AspectRatio;
         Point OldWindowSize;
         RenderTarget2D OffScreenRenderTarget;
         Texture2D BlankTexture;*/
        KeyboardState oldKeyboardState;
        Vector2 mousePosition;
        bool showDebug = false;
        bool isFullScreen = false;

        #region declarations
        RenderTarget2D target;
        RenderTarget2D sceneGray;
        RenderTarget2D sceneColor;
        RenderTarget2D renderAll;

        //int newMapOffset; //transition offset (between 2 screens)

        Vector2 resolution;     //size of the user's screen
        Vector2 screenScale;      //amount to scale the screen 
        Vector2 resolutionOffset; //this will offset the screen in the x direction to account for vertical scaling

        Vector2 oldLevel = new Vector2(1, 1);
        Texture2D[,] background_color = new Texture2D[4,3];
        Texture2D[,] background_gray = new Texture2D[4,3];
        Texture2D[,] parallax = new Texture2D[4, 3];

        int dissolveNum = 10;
        Vector2[] auraDissolve_array = new Vector2[10];

        Texture2D _AlphaMask, cuttingDot, healthCircle, infectionTexture, mouseTexture,
            portalTexture, treeTexture, doorTexture, box, particleGood, particleBad, powerupTexture, skillOutlines, skyTexture;

        float time;

        Effect alphaEffect, darkenEffect;

        public MouseState mouse = new MouseState();

        SpriteFont portalFont;

        TileMap myMap = new TileMap(); 

        //List<Rectangle> dummyListForOverlayToHoldNoPortals = new List<Rectangle>();

        List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();
        List<Rectangle> collisionGround = new List<Rectangle>();
        List<Rectangle> overlayList = new List<Rectangle>();
        List<Enemy> enemies = new List<Enemy>();
        Vector2 levelNumber = new Vector2(1, 1);
        
        int squaresAcross = 192;
        int squaresDown = 108;
        float holeSize = 1f;
        float auraSizeOffset = 0;
        float auraSizeOffsetOld = 0;
        public bool hasAura = false;
        //ParticleEmitter emitter;
        Infection infection = new Infection();
        //float cooldownTimer = 0;
        //float maxActiveTime = 0;
        //float skillRotation = 0f;

        //bool gameOver = false;
        GameOver gameOv;
        MouseState oldMouse;
        
        //determine the gamestate
        public enum GameState { MainMenu, Active, Pause, GameOver};
        public enum SubGameState { None, Dialogue, Cutscene };
        GameState currentState = GameState.MainMenu;
        SubGameState subGameState = SubGameState.None;

        Viewport view;
        
        Song level1;
        Song level2;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            Mouse.WindowHandle = this.Window.Handle;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            target = new RenderTarget2D(graphics.GraphicsDevice, 1920,1080);
            sceneGray = new RenderTarget2D(graphics.GraphicsDevice, 1920, 1080);
            sceneColor = new RenderTarget2D(graphics.GraphicsDevice, 1920, 1080);
            renderAll = new RenderTarget2D(graphics.GraphicsDevice, 1920, 1080);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ChangeResolution(new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Camera.location = new Vector2(0, 80);
            view = GraphicsDevice.Viewport;
            

            alphaEffect = Content.Load<Effect>("Effects/AlphaMap");
            darkenEffect = Content.Load<Effect>("Effects/DarkenDistance");


            #region Load Textures
            //background_color = Content.Load<Texture2D>(@"Textures\Backgrounds\Level1\level1_1");
            //background_gray = Content.Load<Texture2D>(@"Textures\Backgrounds\Level1\level1_1");
            Tile.TileSetTexture = Content.Load<Texture2D>(@"Textures\TileSets\tileSheetBig"); //load map
            cuttingDot = Content.Load<Texture2D>("Textures/UI/auraNew");
            portalTexture = Content.Load<Texture2D>(@"Textures\portal");
            mouseTexture = Content.Load<Texture2D>(@"Textures\UI\cursor");
            infectionTexture = Content.Load<Texture2D>(@"Textures\infection");
            portalFont = Content.Load<SpriteFont>(@"portalFont");
            treeTexture = Content.Load<Texture2D>(@"Textures\Environment\tree");
            healthCircle = Content.Load<Texture2D>(@"Textures\UI\healthCircle");
            doorTexture = Content.Load<Texture2D>(@"Textures\TileSets\door");
            box = Content.Load<Texture2D>("box");
            particleGood = Content.Load<Texture2D>("Particle");
            particleBad = Content.Load<Texture2D>("Particle2");
            powerupTexture = Content.Load<Texture2D>(@"Textures\Environment\powerupSheet");
            skillOutlines = Content.Load<Texture2D>(@"Textures\Sprites\auracolorsheet");
            #endregion

            // load first map
            LoadMap(1, Vector2.Zero);

            
            
            MainMenu.LoadContent(Content, GraphicsDevice);
            Character.LoadContent(Content);
            UserInterface.LoadContent(Content);
            Character.colorMap = Tile.TileSetTexture;
            InitEnemies(); // initialize enemies
            SkillBar.LoadContent(Content, view);
            PowerupInterface.LoadContent(Content, view);
            SkillBar.activeSkill = new Powerup(Vector2.Zero, Vector2.Zero, -1, "none","none", 0, 0);

            level1 = Content.Load<Song>(@"Music\level1");
            level2 = Content.Load<Song>(@"Music\level2");
            MediaPlayer.Play(level1);
            MediaPlayer.Volume = 0f;
            MediaPlayer.IsRepeating = true;

            for (int x = 0; x < dissolveNum;x++)
            {
                auraDissolve_array[x] = Character.position;
            }
        }

        private void ChangeResolution(Vector2 newResolution, bool isFull)
        {
            
            isFullScreen = isFull;
            resolution = newResolution;
            float aspectRatio = 16f/9f;
            
            int height = (int)newResolution.Y;
            int width = (int)((height)*(aspectRatio)); //calculate new width to keep 16:9 aspect ratio
            resolution = new Vector2(width, height);
            graphics.PreferredBackBufferWidth = (int)newResolution.X;
            graphics.PreferredBackBufferHeight = height;

            if (isFullScreen)
                graphics.IsFullScreen = true;
            else
                graphics.IsFullScreen = false;

            float scale = height / 1080f;

            resolutionOffset = new Vector2((1920f - (int)(newResolution.X / scale)) / 2, 0);              //width - newResolution.X) / 2, 0); //amount to offset the screen in the negative 
                                                   //X direction to account for scaling on different aspect ratios.

            graphics.ApplyChanges();

            screenScale.Y = scale;
            screenScale.X = scale; //width / 1920f;
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
           

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// 
            
        /// 
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            time = (float)gameTime.ElapsedGameTime.Milliseconds;
            view = GraphicsDevice.Viewport;

            //if (!this.IsActive && currentState == GameState.Active)
            //    currentState = GameState.Pause;
            //else if (this.IsActive && currentState == GameState.Pause)
            //    currentState = GameState.Active;
            
            switch (currentState)
            {
                case GameState.MainMenu:
                    {
                        mouse = Mouse.GetState();
                        Vector2 resoScale = new Vector2(1920 / resolution.X, 1080 / resolution.Y);
                        mousePosition = Camera.ScreenToWorld(new Vector2(mouse.X * resoScale.X, mouse.Y * resoScale.Y));
                        switch (MainMenu.Update(mouse, mousePosition, this.Window, screenScale))
                        {
                            case -1:
                                currentState = GameState.Active;
                                this.IsMouseVisible = false;
                                break;
                            case 0:
                                break;
                            default:
                                {
                                    //change window size
                                }
                                break;
                        }
                    }
                    break;
                case GameState.Active:
                    Active(gameTime);
                    break;
                case GameState.Pause:
                    Pause();
                    break;
                case GameState.GameOver:
                    GameOver(gameTime);
                    break;
                default:
                    this.Exit();
                    break;
            }
        }
        
        private void AuraEffect(Vector2 RemovePosition)
        {
            for (int i = 1; i < dissolveNum - 1; i++)
            {
                auraDissolve_array[i] = auraDissolve_array[i + 1];
            }
            auraDissolve_array[dissolveNum - 1] = Character.position;
            
            // set the RenderTarget2D as the target for all future Draw calls untill we say otherwise
            graphics.GraphicsDevice.SetRenderTarget(target);

            // start with a transparent canvas
            graphics.GraphicsDevice.Clear(Color.Transparent);

            float powerOffset;

            if (Character.currentPowerup == Character.powerUp.Aura)
                powerOffset = 1.5f;
            else
                powerOffset = 0f;
            
            // start our batch as usual..
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp,null,null);
            
            //if(Character.currentPowerup == (Character.powerUp)2)//tiles
            //    foreach (Vector2 tile in Character.hiddenTiles)
            //    {
            //        spriteBatch.Draw(cuttingDot,
            //        new Vector2(tile.X + 20,tile.Y + 15),
            //        null,
            //        Color.White,
            //        0f,
            //        new Vector2(cuttingDot.Width / 2f, cuttingDot.Height / 2f),
            //        .3f,
            //        SpriteEffects.None,
            //        1f);
            //    }
            
            Color colorTest = Color.White;
            
            if (hasAura)
            {
                for (int j = dissolveNum - 1; j >= 0;j--)
                {
                    if (j == 0)
                    {
                        colorTest.A = 100;
                        spriteBatch.Draw(cuttingDot,
                            Camera.WorldToScreen(RemovePosition),
                            null,
                            colorTest,
                            0f,
                            new Vector2(cuttingDot.Width / 2f, cuttingDot.Height / 2f),
                            holeSize - auraSizeOffset * holeSize + powerOffset,
                            SpriteEffects.None,
                            1f);
                    }
                    else
                    {
                        colorTest.A = (byte)(j * 10);
                        spriteBatch.Draw(cuttingDot,
                            Camera.WorldToScreen(auraDissolve_array[j] + new Vector2(20,30)),
                            null,
                            colorTest,
                            0f,
                            new Vector2(cuttingDot.Width / 2f, cuttingDot.Height / 2f),
                            holeSize - auraSizeOffset * holeSize + powerOffset,
                            SpriteEffects.None,
                            1f);
                    }
                    colorTest.A = 20;
                    // add a new dot to the map. 
                    
                }
            }

            foreach (Enemy enemy in enemies)
            {
                if (enemy.enemyHasAura && myMap.mapNumber == enemy.levelNumber)
                {
                    spriteBatch.Draw(cuttingDot,
                    Camera.WorldToScreen(enemy.position + new Vector2(20,30)),
                    null,
                    Color.White,
                    0f,
                    new Vector2(cuttingDot.Width / 2f, cuttingDot.Height / 2f),
                    holeSize - auraSizeOffset * holeSize,
                    SpriteEffects.None,
                    1f);
                }
            }
            spriteBatch.End();


            // start drawing to the screen again
            graphics.GraphicsDevice.SetRenderTarget(null);

            // set our Texture2D Alpha Mask to equal the current render target (the new mask).
            // RenderTarget2D can be cast to a Texture2D without a problem
            _AlphaMask = target;
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    MainMenu.Draw(spriteBatch,screenScale);
                    break;
                case GameState.Active:
                    DrawActive();
                    break;
                case GameState.Pause:
                    DrawActive();
                    break;
                case GameState.GameOver:
                    DrawActive();
                    break;
                default:
                    this.Exit();
                    break;
            }
            
            base.Draw(gameTime);
        }

        private void DrawActive()
        {
            Vector2 offset = new Vector2(
                    Character.position.X + 20,
                    Character.position.Y + 30);

            AuraEffect(offset);
            //DRAW GRAY///////////////////////////////////////////////////////////////////////////////////

            graphics.GraphicsDevice.SetRenderTarget(sceneGray);
            spriteBatch.Begin(); // draw gray scene with gray tiles to sceneGray renderTarget

            spriteBatch.Draw(skyTexture, Vector2.Zero, Color.White);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    spriteBatch.Draw(parallax[i, j],
                            Camera.WorldToScreen(new Vector2(i * 1920 - 1920, j * 1080 + 200) + Character.parallaxOffset),//(Character.position - new Vector2(0, 3240)) / 3f),
                            background_gray[i, j].Bounds,
                            Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f); // draw parallax
                }

            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Vector2.Distance(Character.position, new Vector2(1920 * i + 1920 / 2, 1080 * j + 1080 / 2)) < 4000)
                    {
                        spriteBatch.Draw(background_gray[i, j],
                            Camera.WorldToScreen(new Vector2(i * 1920, j * 1080)),
                            background_gray[i, j].Bounds,
                            Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f); // draw background gray
                    }
                }
            }
            
            foreach (MovingPlatform plat in movingPlatforms)
            {
                spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(plat.position), Tile.GetSourceRectangle(98), Color.White);
            }
            foreach (Enemy en in enemies)
                en.Draw(spriteBatch);
            DrawPortals(spriteBatch);
            if (infection.isActive)
                infection.Draw(spriteBatch, infectionTexture);
            
            DrawMap(myMap);
            Character.Draw(spriteBatch);
            DrawOverlayGray();

            int colorOffset = (int)Character.currentPowerup;
            
            /*if (hasAura && Character.currentPowerup != Character.powerUp.None)
            {
                spriteBatch.Draw(skillOutlines,
                    Camera.WorldToScreen(
                        offset - new Vector2(5, 5)),
                        new Rectangle(310 * (colorOffset - 1), 0, 310, 310),
                        Color.White,
                        0f,
                        new Vector2(cuttingDot.Width / 2f, cuttingDot.Height / 2f),
                        holeSize - auraSizeOffset * holeSize,
                        SpriteEffects.None,
                        1f);
            }*/
            /*SkillBar.DrawSkillBar(spriteBatch, view);
            if (PowerupInterface.isOpen)
                PowerupInterface.Draw(spriteBatch);
            */

            UserInterface.Draw(spriteBatch);

            /*Point pos = new Point(Convert.ToInt32(mouse.X), Convert.ToInt32(mouse.Y));
            if (PowerupInterface.isOpen)
            {
                string text = PowerupInterface.CheckHover(pos);
                if (text != null)
                {
                    HoverText.Draw(portalFont, text, spriteBatch, view, box);
                }
            }*/
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            //DRAW COLOR///////////////////////////////////////////////////////////////

            GraphicsDevice.SetRenderTarget(sceneColor);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(skyTexture, Vector2.Zero, Color.White);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    spriteBatch.Draw(parallax[i, j],
                                        Camera.WorldToScreen(new Vector2(i * 1920 - 1920, j * 1080 + 200) + Character.parallaxOffset),
                                        background_gray[i, j].Bounds,
                                        Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f); // draw parallax
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Vector2.Distance(Character.position, new Vector2(1920 * i + 1920 / 2, 1080 * j + 1080 / 2)) < 4000)
                    {
                        spriteBatch.Draw(background_color[i, j],
                            Camera.WorldToScreen(new Vector2(i * 1920, j * 1080)),
                            background_gray[i, j].Bounds,
                            Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f); // draw background color
                    }
                }
            }
            
            Rectangle playerRect = new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60); // rectangle for player used with spritefont drawing

            foreach (MovingPlatform plat in movingPlatforms)
            {
                spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(plat.position), Tile.GetSourceRectangle(98), Color.White);
            }

            if (hasAura)
            {
                DrawOverlayColor();
            }

            #region draw portals, infection, character, and UI

            if (infection.isActive)
                infection.Draw(spriteBatch, infectionTexture);

            UserInterface.DrawProjectiles(spriteBatch);

            DrawPortals(spriteBatch);

            float rotation = ((Character.position.X / view.Width) * 2 - 1) * -1;
            //if (!hasAura)
            //{

            //    Character.Draw(spriteBatch);
            //}

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            Character.DrawPowerups(spriteBatch, new Vector2(myMap.mapNumber, myMap.submapNumber), powerupTexture);

            SkillBar.DrawSkillBar(spriteBatch, view);
            if (PowerupInterface.isOpen)
                PowerupInterface.Draw(spriteBatch);
            Character.Draw(spriteBatch);
            Character.DrawTiles(spriteBatch); // drawing the colored background and colored tiles behind everything to get aura effect
            UserInterface.Draw(spriteBatch);
            /*Point pos = new Point(Convert.ToInt32(mouse.X), Convert.ToInt32(mouse.Y));
            if (PowerupInterface.isOpen)
            {
                string text = PowerupInterface.CheckHover(pos);
                if (text != null)
                {
                    HoverText.Draw(portalFont, text, spriteBatch, view, box);
                }
            }
            */
            //emitter.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin();
            graphics.GraphicsDevice.SetRenderTarget(null);
            if(myMap.mapNumber == 2)
            graphics.GraphicsDevice.SetRenderTarget(renderAll);

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //Vector2 tempVector;
            //if (resolution.X == 1920)
            //    tempVector = new Vector2(0, 0);
            //else
            //    tempVector = new Vector2(1, 0);

            spriteBatch.Draw(sceneColor, Vector2.Zero - resolutionOffset , null, Color.White, 0f, Vector2.Zero, screenScale, SpriteEffects.None, 1f);

            spriteBatch.End();
            // set the Mask to use for our shader.
            // note that "MaskTexture" corresponds to the public variable in AlphaMap.fx
            alphaEffect.Parameters["MaskTexture"].SetValue(_AlphaMask);
            
            // start drawing hole in the sceneGray renderTarget of gray scene
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, null, null, alphaEffect);

            spriteBatch.Draw(sceneGray,
                Vector2.Zero - resolutionOffset,
                null, Color.White, 0f, Vector2.Zero,
                screenScale, SpriteEffects.None, 1f);
            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.End();

            if (myMap.mapNumber == 2)
            {
                darkenEffect.Parameters["LightPosition"].SetValue(new Vector2(20, 50));
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    null, null, null, darkenEffect);
                spriteBatch.Draw(renderAll,
                    Vector2.Zero - resolutionOffset,
                    null, Color.White, 0f, Vector2.Zero,
                    screenScale, SpriteEffects.None, 1f);
                spriteBatch.End();
            }

            
            spriteBatch.Begin();
            spriteBatch.Draw(mouseTexture, new Vector2(mouse.X - mouseTexture.Width / 2, mouse.Y - mouseTexture.Height / 2), Color.White);
            if (currentState == GameState.GameOver || gameOv != null && gameOv.fadeout)
                gameOv.Draw(spriteBatch, screenScale);
            
            // draw debug
            if (showDebug)
            {
                spriteBatch.Draw(box, new Rectangle(10, 200, 200, 270), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                spriteBatch.DrawString(portalFont, "Aura Size: " + 300 * holeSize, new Vector2(10, 200), Color.Green);
                spriteBatch.DrawString(portalFont, "X position: " + Character.position.X.ToString(), new Vector2(10, 220), Color.Green);
                spriteBatch.DrawString(portalFont, "Y position: " + Character.position.Y.ToString(), new Vector2(10, 240), Color.Green);
                foreach (Powerup power in SkillBar.skillList)
                {
                    spriteBatch.DrawString(portalFont, "Cooldown " + power.powerType + ": " + (int)power.cooldownTimer, new Vector2(10, 260 + 40 * power.powerNum), Color.Green);
                    spriteBatch.DrawString(portalFont, "Duration " + power.powerType + ": " + (int)power.duration, new Vector2(10, 280 + 40 * power.powerNum), Color.Green);
                }
                if (Character.stuckToWall)
                    spriteBatch.DrawString(portalFont, "stuckToWall = true", new Vector2(10, 320), Color.Green);
                if (Character.hitTop)
                    spriteBatch.DrawString(portalFont, "hitTop", new Vector2(10, 340), Color.Green);

            }

            switch (myMap.mapNumber)
            {
                case 1:
                    Scripts.Level1.Draw(spriteBatch);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }

            spriteBatch.End();

            #endregion
        }

        //private void DrawTransition(SpriteBatch spriteBatch, Viewport view)
        //{
        //    //RenderTarget2D oldMap;//transition effect old
        //    //RenderTarget2D newMap;//transition effect new
        //    //Vector2 newMapOffset; //transition offset (between 2 screens)
        //    //bool isTransitioning = false;
        //    //Vector2 oldLevel = new Vector2(1, 1);

        //    //graphics.GraphicsDevice.SetRenderTarget();



        //    spriteBatch.Begin();

        //    spriteBatch.Draw(sceneGray,
        //           Camera.WorldToScreen(new Vector2(newMapOffset - view.Width, 0)),
        //           null, Color.White, 0f, Vector2.Zero,
        //           1f, SpriteEffects.None, 1f);
        //    spriteBatch.Draw(sceneGray,
        //           new Vector2(newMapOffset + view.Width, 0),
        //           null, Color.White, 0f, Vector2.Zero,
        //           1f, SpriteEffects.None, 1f);
           

        //    Character.Draw(spriteBatch);

        //    for (int i = 0; i < 3; i++)
        //    {
        //        for(int j = 0; j < 4; j++)
        //        {
        //            if(Vector2.Distance(Character.position, new Vector2(1920*j, 1080*i)) < 2000)
        //            spriteBatch.Draw(background_gray[i,j],
        //                new Vector2(newMapOffset, 0),
        //                null, Color.White, 0f, Vector2.Zero,
        //                1f, SpriteEffects.None, 1f);
        //        }
        //    }
            

        //    int tileID;

        //    for (int i = 0; i < collisionGround.Count; i++)
        //    {
        //        tileID = collisionGround[i].Width;

        //        if (tileID < 43)
        //            spriteBatch.Draw(Tile.TileSetTexture, new Vector2( //use width to store TileID =P
        //                        collisionGround[i].X + newMapOffset, collisionGround[i].Y), Tile.GetSourceRectangle(collisionGround[i].Width + (Tile.TileSetTexture.Width / Tile.TileWidth) - 1),
        //                        Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        //        else if (tileID > 45)
        //            spriteBatch.Draw(Tile.TileSetTexture, new Vector2( //use width to store TileID =P
        //                        collisionGround[i].X + newMapOffset, collisionGround[i].Y), Tile.GetSourceRectangle(collisionGround[i].Width - 1),
        //                        Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

        //    }

        //    spriteBatch.End();

            
        //}

        public List<Rectangle> collisionRect
        {
            get { return collisionGround; }
        }

        public void LoadMap(int mapNum, Vector2 position)
        {
            switch (mapNum)
            {
                case 1:
                    Scripts.Level1.LoadContent(Content);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }

            levelNumber = new Vector2(mapNum);
            List<Rectangle> portal = new List<Rectangle>();
            Character.grassList.Clear();
            myMap.portals.Clear();
            Character.parallaxOffset = Vector2.Zero;
            Character.playerProjectiles.Clear();
            myMap.LoadMap(mapNum, ref background_color, ref background_gray, ref parallax, Content, ref movingPlatforms);
            collisionGround.Clear();
            infection.isActive = false;
            skyTexture = Content.Load<Texture2D>(@"Textures\Backgrounds\Level" + mapNum + @"\Sky\skyclouds");

            for (int y = 0; y < squaresDown; y++)
            {
                for (int x = 0; x < squaresAcross; x++)
                {
                    foreach (int tileID in myMap.Rows[y].Columns[x].BaseTiles)
                    {
                        if (tileID > 0 && tileID != (7*(Tile.TileSetTexture.Width/40) + 1))
                            collisionGround.Add(new Rectangle(
                                    (x * Tile.TileWidth), (y * Tile.TileHeight),
                                    tileID, Tile.TileHeight));
                    }
                }
            }
            Character.collisionRect = collisionGround;
        }

        public void DrawMap(TileMap map)
        {
            int tileID;

            for (int i = 0; i < collisionGround.Count; i++)
            {
                tileID = collisionGround[i].Width;
                int tileSetWidth = Tile.TileSetTexture.Width / 40;

                if (tileID < (tileSetWidth * 6 + 0))

                    spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(new Vector2(collisionGround[i].X, collisionGround[i].Y)), Tile.GetSourceRectangle(collisionGround[i].Width + (Tile.TileSetTexture.Width / Tile.TileWidth) - 1),
                                Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                else if (tileID > (tileSetWidth * 6 + 2))
                    spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(new Vector2(collisionGround[i].X, collisionGround[i].Y)), Tile.GetSourceRectangle(collisionGround[i].Width - 1),
                                Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                if(showDebug)
                spriteBatch.DrawString(portalFont, tileID.ToString(), Camera.WorldToScreen(new Vector2(collisionGround[i].X, collisionGround[i].Y)), Color.White);
            }

        }
        public void DrawOverlayGray()
        {
            Color alpha = Color.White;

            for (int y = 0; y < 108; y++)
            {
                for (int x = 0; x < 192; x++)
                {
                    if (int.Parse(myMap.overlayMap[x, y]) > 0)
                    {
                        spriteBatch.Draw(Tile.TileSetTexture,
                            Camera.WorldToScreen(new Vector2(x * Tile.TileWidth, y * Tile.TileHeight)),
                            Tile.GetSourceRectangle(int.Parse(myMap.overlayMap[x, y]) - 1),
                            alpha, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    }
                }
            }
        }
        public void DrawOverlayColor()
        {
            for (int y = 0; y < 108; y++)
            {
                for (int x = 0; x < 192; x++)
                {

                    if (int.Parse(myMap.overlayMap[x, y]) > 0)
                    {

                        if (int.Parse(myMap.overlayMap[x, y]) >= 113 && int.Parse(myMap.overlayMap[x, y]) <= 224)
                        {
                            int tilesWide = Tile.TileSetTexture.Width / 40;
                            spriteBatch.Draw(Tile.TileSetTexture,
                                Camera.WorldToScreen(new Vector2(x * Tile.TileWidth, y * Tile.TileHeight)),
                                Tile.GetSourceRectangle((int.Parse(myMap.overlayMap[x, y]) - 1) + (8 * tilesWide)),
                                Color.Wheat, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }
        public void DrawPortals(SpriteBatch spriteBatch)
        {
            foreach (Rectangle portalRect in myMap.portals)
            {
                if ((portalRect.Width == 2 || portalRect.Width == 1) && myMap.mapNumber == 2)
                {
                    //spriteBatch.Draw(doorTexture, new Vector2(portalRect.X * Tile.TileWidth, portalRect.Y * Tile.TileHeight - 30), Color.White);
                }
                else
                spriteBatch.Draw(portalTexture, Camera.WorldToScreen(new Vector2(portalRect.X * Tile.TileWidth, portalRect.Y * Tile.TileHeight - 30)), Color.White);

                if (new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60).Intersects(new Rectangle(portalRect.X * Tile.TileWidth, portalRect.Y * Tile.TileHeight - 30, 40, 60)))
                {
                    spriteBatch.DrawString(portalFont, "Press 'e' to \nload level " + portalRect.Width, Camera.WorldToScreen(new Vector2(portalRect.X * Tile.TileWidth - 20, portalRect.Y * Tile.TileHeight - 120)), Color.Black);
                }
            }
            
        }

        public void InitEnemies()
        {
            XmlDocument xmlDoc = new XmlDocument(); // load enemies from xml
            xmlDoc.Load(@"Content\Maps\Enemies.xml");
            XmlNodeList enemyList = xmlDoc.GetElementsByTagName("Item");

            foreach (XmlNode node in enemyList)
            {
                Enemy tempEnemy = new Enemy(Content, particleBad, int.Parse(node.ChildNodes.Item(2).InnerText));
                
                string[] position = node.ChildNodes.Item(0).InnerText.Split();
                string level = node.ChildNodes.Item(1).InnerText;
                tempEnemy.enemyType = int.Parse(node.ChildNodes.Item(2).InnerText);
                int numDialogue = int.Parse(node.ChildNodes.Item(3).InnerText);

                if (numDialogue != 0)
                {
                    for (int i = 0; i < numDialogue; i++)
                    {
                        string dialogue = node.ChildNodes.Item(4 + i).InnerText;
                        tempEnemy.dialogue.Add(dialogue);
                    }
                }
                tempEnemy.levelNumber = int.Parse(level);
                tempEnemy.position = new Vector2(int.Parse(position[0]) * 40, int.Parse(position[1]) * 30 - 30 + 1);
                if (tempEnemy.enemyType == 0)
                {
                    tempEnemy.velocity.X = 5f;
                    tempEnemy.startPosition = tempEnemy.position;
                }
                if (tempEnemy.enemyType == 3)
                    tempEnemy.enemyHasAura = true;
                enemies.Add(tempEnemy);
            }
        }

        bool intersects(Rectangle circle, Rectangle rect)
        {
            
            Vector2 circleDistance;
            circleDistance.X = Math.Abs(circle.X+circle.Width/2 - rect.X - rect.Width/2);
            circleDistance.Y = Math.Abs(circle.Y+circle.Width/2 - rect.Y - rect.Height/2);

            if (circleDistance.X > rect.Width / 2 + circle.Width / 2) return false;
            if (circleDistance.Y > rect.Height / 2 + circle.Height / 2) return false;

            if (circleDistance.X <= (rect.Width / 2)) { return true; }
            if (circleDistance.Y <= (rect.Height / 2)) { return true; }

            double crnrDistance_sq = Math.Pow((circleDistance.X - rect.Width / 2), 2) + Math.Pow((circleDistance.Y - rect.Height / 2), 2);
            return crnrDistance_sq <= (Math.Pow(circle.Width/2,2));
        }
        

        void Active(GameTime gameTime)
        {
            oldMouse = mouse;
            KeyboardState ks = Keyboard.GetState();
            Mouse.WindowHandle = this.Window.Handle;
            mouse = Mouse.GetState();
            Vector2 resoScale = new Vector2(1920/resolution.X, 1080/resolution.Y);
            mousePosition = Camera.ScreenToWorld(new Vector2(mouse.X * resoScale.X, mouse.Y * resoScale.Y));
           
            // pulse aura

            switch(subGameState)
            {
                case SubGameState.None:
                    {

                        switch (myMap.mapNumber)
                        {
                            case 1:
                                Scripts.Level1.Update(ks, ref subGameState, gameTime);
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            case 6:
                                break;
                        }

                        if (auraSizeOffset == 0 || (auraSizeOffset > auraSizeOffsetOld && auraSizeOffset < .05f))
                        {
                            auraSizeOffsetOld = auraSizeOffset;
                            auraSizeOffset += .0025f;
                        }
                        else
                        {
                            auraSizeOffsetOld = auraSizeOffset;
                            auraSizeOffset -= .0025f;
                        }

                        //calculate aura's rectangle based on size of aura and chartacters position
                        Rectangle auraRect = new Rectangle(
                            ((int)Character.position.X + 20) - (int)((300 * holeSize) / 2),
                            ((int)Character.position.Y + 30) - (int)((300 * holeSize) / 2),
                             (int)(300 * holeSize) - 20,
                             (int)(300 * holeSize) - 20);

                        foreach (Enemy enemy in enemies)
                        {
                            enemy.currentLevel = myMap.mapNumber;
                            if (enemy.levelNumber == enemy.currentLevel)
                            {
                                Rectangle enemyRect = new Rectangle((int)enemy.position.X, (int)enemy.position.Y, 40, 60);
                                if (enemy.health > 0 && enemy.enemyType == 0)
                                {
                                    enemy.Patrol(collisionGround);
                                    enemy.Update(gameTime, view, levelNumber, Content, holeSize, ref hasAura);

                                    if (intersects(auraRect, enemyRect))
                                    {
                                        enemy.health -= 5;
                                    }
                                }
                                else
                                {
                                    if (enemy.enemyType == 0)
                                        enemy.enemyType = 1;
                                    enemy.Update(gameTime, view, new Vector2(myMap.mapNumber, myMap.submapNumber), Content, holeSize, ref hasAura);
                                }
                            }
                        }

                        //debug menu
                        if (ks.IsKeyDown(Keys.F8) && ks.IsKeyDown(Keys.F8) != oldKeyboardState.IsKeyDown(Keys.F8))
                        {
                            if (showDebug)
                            {
                                showDebug = false;
                            }
                            else
                            {
                                showDebug = true;
                            }
                        }

                        // Allows the game to exit
                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || ks.IsKeyDown(Keys.Escape) && ks.IsKeyDown(Keys.Escape) != oldKeyboardState.IsKeyDown(Keys.Escape))
                            currentState = GameState.Pause;


                        #region check portals for input
                        Rectangle playerRect = new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60);
                        PlayerIndex playerIndex = new PlayerIndex();

                        //Vector2 levelNumber = new Vector2(myMap.mapNumber, myMap.submapNumber);
                        foreach (Rectangle portalRect in myMap.portals)
                        {
                            if (playerRect.Intersects(new Rectangle(portalRect.X * Tile.TileWidth, portalRect.Y * Tile.TileHeight - 30, 40, 60))
                                && (ks.IsKeyDown(Keys.E) || GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed)
                                && (ks.IsKeyDown(Keys.E) != oldKeyboardState.IsKeyDown(Keys.E))
                                && (levelNumber != new Vector2(2, 4) || hasAura))
                            {

                                try
                                {
                                    int level = portalRect.Width;
                                    Vector2 position = Vector2.Zero;
                                    switch (level)
                                    {
                                        case 1:
                                            {
                                                if (myMap.mapNumber == 2)
                                                    position = new Vector2(12 * Tile.TileWidth, 21 * Tile.TileHeight);
                                                MediaPlayer.Play(level1);
                                                break;
                                            }
                                        case 2:
                                            {
                                                MediaPlayer.Play(level2);
                                                break;
                                            }
                                        case 3: MediaPlayer.Play(level1);
                                            break;
                                    }
                                    foreach (Powerup power in SkillBar.skillList)
                                        power.isStuck = true;
                                    oldLevel = new Vector2(level, 1);
                                    SkillBar.activeSkill = new Powerup(Vector2.Zero, Vector2.Zero, -1, "none", "none", 0, 0);
                                    Character.currentPowerup = (Character.powerUp)0;
                                    //maxActiveTime = 0;
                                    Character.powerUpList.Clear();
                                    Character.usedPowerUps.Clear();
                                    Character.position = position;
                                    LoadMap(level, position);
                                }
                                catch
                                {
                                    //level doesnt exist, you should prob restart the game
                                }
                                break;
                            }
                        }
                        #endregion

                        #region Aura Update
                        Vector2 offset = new Vector2(
                            Character.position.X + 20,
                            Character.position.Y + 30);

                        holeSize = (float)Character.Health / 100f + .2f; //set hole size and clamp it
                        if (holeSize < .3f)
                            holeSize = .3f;
                        if (holeSize > 1)
                            holeSize = 1;

                        //if (hasAura)              //draw aura if player has obtained it
                        AuraEffect(offset);
                        #endregion

                        List<Rectangle> platRects = new List<Rectangle>();
                        if (movingPlatforms != null) //update all moving platforms
                            UpdateMovingPlatforms();
                        foreach (MovingPlatform plat in movingPlatforms)
                        {
                            platRects.Add(new Rectangle((int)plat.position.X, (int)plat.position.Y, 40, 30));
                        }

                        Character.Update(gameTime, GraphicsDevice, collisionRect, platRects, hasAura, new Vector2(myMap.mapNumber, myMap.submapNumber), view, resolutionOffset, mouse, mousePosition); //update the player

                        if (infection.isActive)
                        {
                            infection.Update(gameTime);

                            if (infection.rect.Intersects(new Rectangle((int)Character.position.X, (int)Character.position.Y, 40, 60)))
                                Character.Health -= .5f;
                        }

                        if (Character.Health <= 0)
                        {

                            currentState = GameState.GameOver;
                            gameOv = new GameOver(Content);
                        }
                    }
                    break;

                //CUTSCENE
                case SubGameState.Cutscene:
                {
                    switch (myMap.mapNumber)
                    {
                        case 1:
                            Scripts.Level1.Update(ks, ref subGameState, gameTime);
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                        case 6:
                            break;
                    }
                }
                break;

            }

            /*if (mouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != oldMouse.LeftButton && !SkillBar.isCollapsing && !SkillBar.isExpanding)
           {
               //Enemy enemy = new Enemy(Content, particleGood, particleBad, 0, Camera.ScreenToWorld(new Vector2(mouse.X, mouse.Y)), myMap.mapNumber);
               //enemies.Add(enemy);

               Point temp = new Point(Convert.ToInt32(mouse.X), Convert.ToInt32(mouse.Y));
               if (PowerupInterface.isOpen)
               {
                   //if (PowerupInterface.interfaceRect.Contains(temp))
                   //{
                       PowerupInterface.CheckClick(temp, screenScale);

                   SkillBar.CheckClick(temp, view);
               }
           }*/

            //INPUTS
            //if (ks.IsKeyDown(Keys.D1) && ks.IsKeyDown(Keys.D1) != oldKeyboardState.IsKeyDown(Keys.D1) && SkillBar.skillList.Count >= 1 && !SkillBar.skillList[0].onCooldown)
            //{
            //    Character.currentPowerup = (Character.powerUp)SkillBar.skillList[0].powerNum + 1;
            //    cooldownTimer = 0;
            //    SkillBar.activeSkill = SkillBar.skillList[0];
            //    maxActiveTime = SkillBar.skillList[0].duration;
            //    SkillBar.activeSkill.onCooldown = true;
            //    SkillBar.activeSkill.cooldownTimer = 0f;
            //}
            //if (ks.IsKeyDown(Keys.D2) && ks.IsKeyDown(Keys.D2) != oldKeyboardState.IsKeyDown(Keys.D2) && SkillBar.skillList.Count >= 2 && !SkillBar.skillList[1].onCooldown)
            //{
            //    Character.currentPowerup = (Character.powerUp)SkillBar.skillList[1].powerNum + 1;
            //    cooldownTimer = 0;
            //    SkillBar.activeSkill = SkillBar.skillList[1];
            //    maxActiveTime = SkillBar.skillList[1].duration;
            //    SkillBar.activeSkill.onCooldown = true;
            //    SkillBar.activeSkill.cooldownTimer = 0f;
            //}
            //if (ks.IsKeyDown(Keys.D3) && ks.IsKeyDown(Keys.D3) != oldKeyboardState.IsKeyDown(Keys.D3) && SkillBar.skillList.Count >= 3 && !SkillBar.skillList[2].onCooldown)
            //{
            //    Character.currentPowerup = (Character.powerUp)SkillBar.skillList[2].powerNum + 1;
            //    cooldownTimer = 0;
            //    SkillBar.activeSkill = SkillBar.skillList[2];
            //    maxActiveTime = SkillBar.skillList[2].duration;
            //    SkillBar.activeSkill.onCooldown = true;
            //    SkillBar.activeSkill.cooldownTimer = 0f;
            //}
            //if (ks.IsKeyDown(Keys.D4) && ks.IsKeyDown(Keys.D4) != oldKeyboardState.IsKeyDown(Keys.D4) && SkillBar.skillList.Count >= 4 && !SkillBar.skillList[3].onCooldown)
            //{
            //    Character.currentPowerup = (Character.powerUp)SkillBar.skillList[3].powerNum + 1;
            //    cooldownTimer = 0;
            //    SkillBar.activeSkill = SkillBar.skillList[3];
            //    maxActiveTime = SkillBar.skillList[3].duration;
            //    SkillBar.activeSkill.onCooldown = true;
            //    SkillBar.activeSkill.cooldownTimer = 0f;
            //}
            //if (ks.IsKeyDown(Keys.F) && ks.IsKeyDown(Keys.F) != oldKeyboardState.IsKeyDown(Keys.F))
            //{
            //    if(isFullScreen)
            //        ChangeResolution(new Vector2(1280, 720), false);
            //    else
            //        ChangeResolution(new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), true);
            //}

            //if (Character.currentPowerup != Character.powerUp.None) //not equal none
            //{
            //    maxActiveTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            //}
            //if (maxActiveTime < 0)
            //{
            //    SkillBar.activeSkill = new Powerup(Vector2.Zero, Vector2.Zero, -1, "none", "none", 0, 0);
            //    Character.currentPowerup = (Character.powerUp)0; //set powerup to none
            //    maxActiveTime = 0;
            //}

            //foreach (Powerup power in SkillBar.skillList)
            //{
            //    if (power.onCooldown)
            //    {
            //        power.cooldownTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            //        if (power.cooldownTimer > power.cooldown)
            //            power.onCooldown = false;
            //    }
            //}

            //if (emitter == null)
            //{
            //    emitter = new ParticleEmitter(Vector2.Zero, particle, Color.Brown, Color.MistyRose, new Vector2(0, -1), false);
            //}
            //emitter.Update(gameTime, new Vector2(mouse.X, mouse.Y), false, false);


            // UserInterface.Update(gameTime, GraphicsDevice, mouse);

            
            //if (ks.IsKeyDown(Keys.T) && oldKeyboardState.IsKeyDown(Keys.T) != ks.IsKeyDown(Keys.T)) // whatever activates infection
            //{
            //    infection.NewInfection(gameTime);
            //}

            //if (ks.IsKeyDown(Keys.R) && oldKeyboardState.IsKeyDown(Keys.R) != ks.IsKeyDown(Keys.R)) // whatever activates infection
            //{
            //    PowerupInterface.isOpen = !PowerupInterface.isOpen;
            //}

            //if (ks.IsKeyDown(Keys.Q) && oldKeyboardState.IsKeyDown(Keys.Q) != ks.IsKeyDown(Keys.Q) && SkillBar.skillList.Count > 0)// && !SkillBar.isExpanding && !SkillBar.isCollapsing) // whatever activates infection
            //{
            //    foreach (Powerup powerup in SkillBar.skillList)
            //        powerup.isStuck = false;

            //    if (SkillBar.isActive || SkillBar.isExpanding)
            //    {
            //        SkillBar.isCollapsing = true;
            //        SkillBar.isExpanding = false;
            //        SkillBar.isActive = false;
            //    }
            //    else if (!SkillBar.isActive || SkillBar.isCollapsing)
            //    {
            //        SkillBar.isExpanding = true;
            //        foreach (Powerup powpow in SkillBar.skillList)
            //        {
            //            if (powpow.isStuck)
            //                powpow.position = Character.position + new Vector2(20, 30);
            //            powpow.isStuck = false;
            //        }
            //        if (!SkillBar.isCollapsing)
            //            foreach (Powerup power in SkillBar.skillList)
            //            {
            //                power.position = Character.position + new Vector2(20, 30);
            //            }
            //        SkillBar.isCollapsing = false;
            //    }

            //}
            //if (ks.IsKeyDown(Keys.R) && oldKeyboardState.IsKeyDown(Keys.R) != ks.IsKeyDown(Keys.R) && myMap.mapNumber == 3) // whatever activates infection
            //{
            //    Character.powerUpList.Clear();
            //    Character.usedPowerUps.Clear();
            //    Character.powerUp = "none";
            //    LoadMap(3, 1);
            //    //if (Character.powerUp != "tiles")
            //    //    Character.powerUp = "tiles";
            //    //else
            //    //    Character.powerUp = "none";
            //}

            

            ////check player position to load new submap
            //if (Character.position.X < 10 && myMap.submapNumber != 1) //moving to the left
            //{
            //    float ypos = Character.position.Y;
            //    oldLevel = new Vector2(myMap.mapNumber, myMap.submapNumber);
            //    newMapOffset = -view.Width;
            //    currentState = GameState.Transition;
            //    LoadMap(myMap.mapNumber, Character.position);
            //    if (SkillBar.isCollapsing)
            //    {
            //        SkillBar.isCollapsing = false;
            //        SkillBar.isActive = false;
            //    }
            //    else if (SkillBar.isExpanding)
            //    {
            //        SkillBar.isExpanding = false;
            //        SkillBar.isActive = true;
            //    }
            //    Character.position.Y = ypos;

            //    UserInterface.projectiles.Clear();

            //}
            //else if (Character.position.X > view.Width - 50 && myMap.submapNumber != 4 && myMap.mapNumber != 1) //moving to the right
            //{
            //    oldLevel = new Vector2(myMap.mapNumber, myMap.submapNumber);
            //    newMapOffset = view.Width;
            //    currentState = GameState.Transition;
            //    LoadMap(myMap.mapNumber, Character.position);
            //    if (SkillBar.isCollapsing)
            //    {
            //        SkillBar.isCollapsing = false;
            //        SkillBar.isActive = false;
            //    }
            //    else if (SkillBar.isExpanding)
            //    {
            //        SkillBar.isExpanding = false;
            //        SkillBar.isActive = true;
            //    }

            //    UserInterface.projectiles.Clear();
            //}

            oldKeyboardState = ks;
            base.Update(gameTime);
           
        }

        private void UpdateMovingPlatforms()
        {
            Collision collision = new Collision();

            foreach (MovingPlatform plat in movingPlatforms)
            {
                Rectangle platRect;
                Vector2 velocity;

                if (plat.isMovingRight)
                {
                    plat.position.X += 2;
                    if (Character.isOnMovingPlat)
                        Character.position.X += 2;
                    platRect = new Rectangle((int)plat.position.X, (int)plat.position.Y, 40, 30);
                    velocity = new Vector2(1, 0);
                    collision.EnemyCollisionCheck(collisionGround, ref platRect, ref velocity);
                    if (velocity.X == -1)
                        plat.isMovingRight = false;
                }
                else
                {
                    plat.position.X += -2;
                    if (Character.isOnMovingPlat)
                        Character.position.X += -2;
                    platRect = new Rectangle((int)plat.position.X, (int)plat.position.Y, 40, 30);
                    velocity = new Vector2(-1, 0);
                    collision.EnemyCollisionCheck(collisionGround, ref platRect, ref velocity);
                    if (velocity.X == 1)
                        plat.isMovingRight = true;
                }
            }
            Character.isOnMovingPlat = false;
        }

        void Pause()
        {
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Escape) && keys.IsKeyDown(Keys.Escape) != oldKeyboardState.IsKeyDown(Keys.Escape))
            {
                currentState = GameState.Active;
            }
            else if (keys.IsKeyDown(Keys.Enter))
                this.Exit();
            else if (keys.IsKeyDown(Keys.RightShift))
                currentState = GameState.MainMenu;

            this.IsMouseVisible = true;
            oldKeyboardState = keys;
        }

        void GameOver(GameTime gameTime)
        {
            List<Rectangle> platRects = new List<Rectangle>();

            foreach (MovingPlatform plat in movingPlatforms)
            {
                platRects.Add(new Rectangle((int)plat.position.X, (int)plat.position.Y, 40, 30));
            }

            if (!gameOv.Update(gameTime))
            {
                if (gameOv.alpha == 100 && !gameOv.fadeout)
                {
                    Character.Health = 100;
                    Character.currentPowerup = (Character.powerUp)0;
                    LoadMap(1, Vector2.Zero);
                    hasAura = false;
                    //background_color = Content.Load<Texture2D>(@"Textures\Backgrounds\Level1\level1_1_color");
                    //background_gray = Content.Load<Texture2D>(@"Textures\Backgrounds\Level1\level1_1_gray");
                    enemies.Clear();
                    InitEnemies();
                    movingPlatforms.Clear();
                    Character.playerProjectiles.Clear();
                    Character.playerAnim.CurrentAnimation = "idleEast";

                    Character.Update(gameTime, GraphicsDevice, collisionGround, platRects, hasAura, new Vector2(myMap.mapNumber, myMap.submapNumber), view, resolutionOffset, mouse, mousePosition);
                    currentState = GameState.Active;
                }
            }
            if (gameOv.alpha == 0 && gameOv.goingUp == false)
            {
                Character.Health = 100;
            }
            if (!gameOv.goingUp)
            {
                Character.velocity.Y = 0;
                Character.Update(gameTime, GraphicsDevice, collisionGround, platRects, hasAura, new Vector2(myMap.mapNumber, myMap.submapNumber),view, resolutionOffset, mouse, mousePosition); // move player to correct position in house
            }
        }

        //void Transition(Viewport view)
        //{
        //    if (oldLevel.Y < myMap.submapNumber) //moving right
        //    {

        //        newMapOffset -= 40;
        //        Character.position.X -= 40;

        //        Vector2 check = Vector2.Zero; //(character pos, map position) 1 means done

        //        if (Character.position.X < 20)
        //        {
        //            Character.position.X = 20;
        //            check.X = 1;
        //        }

        //        if (newMapOffset <= 0)
        //        {
        //            check.Y = 1;
        //        }
        //        if (check == new Vector2(1, 1))
        //        {
        //            currentState = GameState.Active;
        //        }
        //    }
        //    else // moving left
        //    {
        //        newMapOffset += 40;
        //        Character.position.X += 40;

        //        Vector2 check = Vector2.Zero; //(character pos, map position) 1 means done

        //        if (Character.position.X > view.Width - 70)
        //        {
        //            Character.position.X = view.Width - 70;
        //            check.X = 1;
        //        }
        //        if (newMapOffset >= 0)
        //        {
        //            check.Y = 1;
        //        }
        //        if (check == new Vector2(1, 1))
        //        {
        //            currentState = GameState.Active;
        //        }
        //    }
        //}
    }
}
