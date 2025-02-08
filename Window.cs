using System;
using System.Linq.Expressions;
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
            Gui.LoadContent(Content);

            // Fonts
            Arial = Content.Load<SpriteFont>("Arial");
            ArialSmall = Content.Load<SpriteFont>("ArialSmall");

            // Widgets
            Gui.Widgets = [
                new Label(Gui, new(100, 100), Color.Red, "Textbox----------", Arial),
                new Input(Gui, new(100, 350), new(300, 25), Color.Black, Color.Gray, Color.LightGray, Arial),
                new InfoBox(Gui, new(100, 400), new(300, 300), new(100, 400, 300, 300), Color.Gray, Color.Black, "Infobox".PadRight(1000, '-'), Arial),
                new HorizontalSlider(Gui, new(100, 725), 100, Color.Black, new(55, 55, 55)),
                new VerticalSlider(Gui, new(100, 750), 100, Color.Black, new(55, 55, 55)),
                new ListBox(Gui, new(650, 50), new(100, 100), Color.Black, Color.Gray, Color.DarkGray),
                new Dropdown(Gui, new(650, 175), new(125, 30), Color.Black, Color.Gray, Color.LightGray, Arial),
                new Button(Gui, new(500, 10), new(100, 30), Color.White, Color.Gray, Color.DarkGray, Widget.NoFunc, text: $"Button---------------", font: Arial),
                new Popup(Gui, new(100, 135), new(200, 200), Color.DarkGray, "Popup Window---------------------------", Arial),
                new Checkbox(Gui, new(650, 250), 25, Color.White, Color.Gray, Color.DarkGray),
            ];
            // Add items
            ((ListBox)Gui.Widgets[5]).AddItems("Item 1--------------------", "Item 2", "Item 3");
            ((Dropdown)Gui.Widgets[6]).AddItems("Selection 1---------------------", "Selection 2", "Selection 3");
            ((Button)Gui.Widgets[7]).Function = (Action)Gui.Widgets[1].Show;
        }

        protected override void Update(GameTime gameTime)
        {
            // Key state and mouse state
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            // Deltatime
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Exit
            if (KeyState.IsKeyDown(Keys.Escape)) { Exit(); }

            // Test

            // Gui
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
            _spriteBatch.End();

            // Base
            base.Draw(gameTime);
        }
    }
}
