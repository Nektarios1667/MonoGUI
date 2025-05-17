using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public Texture2D? CircleOutline { get; private set; }
        public Texture2D? ArrowDown { get; private set; }
        public SpriteFont? Arial { get; private set; }
        private bool _loaded { get; set; }
        public GUI(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            Widgets = [];
            Batch = spriteBatch;
            MouseState = new();
            KeyState = new();
            _loaded = false;
        }
        public void Update(float deltaTime, MouseState mouseState, KeyboardState keyState)
        {
            // Not loaded
            if (!_loaded) { throw new Exception("GUI content needs to be loaded with LoadContent first."); }

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
        public void LoadContent(ContentManager content)
        {
            CircleOutline = content.Load<Texture2D>("CircleOutline");
            ArrowDown = content.Load<Texture2D>("ArrowDown");
            Arial = content.Load<SpriteFont>("Arial");

            _loaded = true;
        }
        // Layers
        public void BringToBack(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.Remove(moving); Widgets.Insert(0, moving); } }
        public void BringToBack(Widget widget) { if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Insert(0, widget); } }
        public void BringToFont(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.Remove(moving); Widgets.Append(moving); } }
        public void BringToFront(Widget widget) { if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Append(widget); } }
        public void BringToIndex(Widget widget, int idx)
        {
            if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Insert(Math.Clamp(idx, 0, Widgets.Count - 1), widget); }
        }
    }
}
