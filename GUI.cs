using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGUI
{
    public class GUI
    {
        public Game Game { get; set; }
        public SpriteBatch Batch { get; private set; }
        public List<Widget> Widgets { get; set; }
        public MouseState MouseState { get; set; }
        public KeyboardState KeyState { get; set; }
        public float Delta { get; private set; }
        public GUI(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            Widgets = [];
            Batch = spriteBatch;
            MouseState = new();
            KeyState = new();
        }
        public void Update(float deltaTime, MouseState mouseState, KeyboardState keyState)
        {
            // Delta time
            Delta = deltaTime;

            // Input
            MouseState = mouseState;
            KeyState = keyState;

            // Updates
            foreach (Widget widget in Widgets) { widget.Update(); }
        }
        public void Draw()
        {
            foreach (Widget widget in Widgets) { widget.Draw(); }
        }
    }
}
