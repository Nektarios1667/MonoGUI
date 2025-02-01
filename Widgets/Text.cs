using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Xna = Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoGUI
{
    public class TextBox : Widget
    {
        public string Text { get; private set; }
        public Xna.Color Color { get; private set; }
        public SpriteFont Font { get; private set; }
        // Centering
        public TextBox(GUI gui, Xna.Vector2 location, Color color, string text, SpriteFont font) : base(gui, location)
        {
            Text = text;
            Font = font;
            Color = color;
        }
        public override void Update() {}
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Text
            Gui.Batch.DrawString(Font, Text, Location, Color);
        }
    }
}
