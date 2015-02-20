using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SadWorld
{
    static public class MainMenu
    {
        
        

        static List<Button> buttons = new List<Button>();
        static RenderTarget2D render;
        static Texture2D background, buttonTextures;

        enum State
        {
            Main,Options
        };

        public struct Button
        {
            public Rectangle rectangle;
            public string state;

            public Button(Rectangle Rectangle,string State)
            {
                state = State;
                rectangle = Rectangle;
            }

        }

        static State currentState = State.Main;

        public static void LoadContent(ContentManager content, GraphicsDevice Graphics)
        {
            

            Button button = new Button(new Rectangle(1920 / 2 - 100, 1080 / 2 - 100, 200, 50), "Main");//start game
            buttons.Add(button);
            button = new Button(new Rectangle(1920 / 2 - 100, 1080 / 2 + 100, 200, 50), "Main");//options
            buttons.Add(button);

            render = new RenderTarget2D(Graphics, 1920, 1080);

            background = content.Load<Texture2D>("Textures/Menu/MainMenuBackground");
            buttonTextures = content.Load<Texture2D>("Textures/Menu/mainOptions");
        }

        public static int Update(MouseState mouse, Vector2 mousePos, GameWindow window, Vector2 screenScale) //return screen width option
                                                       // or 0 for nothing
                                                       // or -1 to start game
        {
            KeyboardState keys = Keyboard.GetState();
            Mouse.WindowHandle = window.Handle;
            

            
            
            if (currentState == State.Main)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    for(int x = 0;x < buttons.Count; x++)
                    {
                        if(buttons[x].state == "Main" && buttons[x].rectangle.Intersects(new Rectangle((int)mousePos.X, (int)mousePos.Y, 1,1)))
                        {
                            return -1;
                            // currentState = State.Options;
                        }
                    }
                }
                else if (keys.IsKeyDown(Keys.Enter))
                    return -1;
            }
            else
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    for (int x = 0; x < buttons.Count; x++)
                    {
                        if (buttons[x].state == "Options" && buttons[x].rectangle.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
                        {
                            return -1;
                            // currentState = State.Options;
                        }
                    }
                }
                
            }

            return 0;
        }

        public static void Draw(SpriteBatch spriteBatch, Vector2 screenScale)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White,0f,Vector2.Zero, screenScale, SpriteEffects.None, 1f);

            for(int x = 0;x < buttons.Count; x++)
            {
                spriteBatch.Draw(buttonTextures, new Vector2(buttons[x].rectangle.X,buttons[x].rectangle.Y) * screenScale, new Rectangle(0, 50 * x, 200, 50), Color.White, 0f, Vector2.Zero, screenScale, SpriteEffects.None, 1f);
            }
            spriteBatch.End();
        }
    }
}
