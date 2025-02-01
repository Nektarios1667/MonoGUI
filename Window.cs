using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGUI
{
    public class Window : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public GUI Gui { get; private set; }
        public KeyboardState KeyState { get; private set; }
        public MouseState MouseState { get; private set; }
        private SpriteFont Arial { get; set; }
        private SpriteFont ArialSmall { get; set; }
        public float DeltaTime { get; private set; }

        public Window()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1400,
                PreferredBackBufferHeight = 900,
                SynchronizeWithVerticalRetrace = false,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Gui = new GUI(this, _spriteBatch);

            // Fonts
            Arial = Content.Load<SpriteFont>("Arial");
            ArialSmall = Content.Load<SpriteFont>("ArialSmall");

            // Widgets
            Gui.Widgets = [
                new TextBox(Gui, new(100, 100), Color.Red, "Textbox", Arial),
                new Popup(Gui, new(100, 300), new(200, 200), Color.DarkGray, "Popup Window", Arial),
                new Input(Gui, new(100, 500), new(100, 40), Color.Black, Color.Gray, Color.LightGray, Arial),
                new Infobox(Gui, new(100, 550), new(300, 300), new(100, 700, 300, 300), Color.Gray, Color.Black, "Infobox", Arial),
                new Button(Gui, new(600, 100), new(100, 40), Color.Black, Color.Gray, Color.LightGray, (Action<string>)Console.WriteLine, text:"Button", font:Arial, args:["Button clicked!"])
            ];
        }

        protected override void Update(GameTime gameTime)
        {
            // Key state and mouse state
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            // Deltatime
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Exit
            if (KeyState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Gui.Update(DeltaTime, MouseState, KeyState);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin
            _spriteBatch.Begin();

            // Corner info
            _spriteBatch.DrawString(Arial, $"Delta: {Math.Round(DeltaTime * 1000)}", new(30, 30), Color.Black);

            // Gui
            Gui.Draw();

            // End
            _spriteBatch.End()
;
            // Base
            base.Draw(gameTime);
        }
    }
}
