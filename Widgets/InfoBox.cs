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
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace MonoGUI
{
    public class InfoBox : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public Rectangle Rect { get; private set; }
        public Xna.Color Color { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        private string _text { get; set; }
        public string Text { get; set; }
        public SpriteFont? Font { get; set; }
        public Color Foreground { get; set; }
        public Xna.Rectangle Activation { get; set; }
        public string Softwrapped { get; set; }
        public Xna.Vector2 Inside { get; set; }
        public float Delay { get; set; }
        private float Time { get; set; }
        // Centering
        public InfoBox(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Xna.Rectangle activation, Color color, Color foreground, string text, SpriteFont? font = default, float delay = 1, int border = 2, Color? borderColor = null) : base(gui, location)
        {
            Dimensions = dimensions;
            Activation = activation;
            Color = color;
            Border = border;
            BorderColor = (borderColor == null ? Color.Black : (Color)borderColor);
            Visible = true;
            _text = text;
            Font = font == default ? gui.Arial : font;
            Foreground = foreground;
            Inside = new(Dimensions.X - Border * 2 - 4, Dimensions.Y - Border * 2 - 4);
            Softwrapped = Font != null ? LimitLines(SoftwrapWords(text, Font, Inside), Font, Inside.Y) : text;
            Delay = delay;
        }
        public override void Update()
        {
            // Hovering
            MouseState mouseState = Gui.MouseState;
            if (PointRectCollide(Activation, mouseState.Position)) {
                if (Time >= Delay) { Visible = true; }
                else { Time += Gui.Delta; }
            }
            else { Visible = false; Time = 0f; }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, Color);
            // Text
            Gui.Batch.DrawString(Font, Softwrapped, new(Location.X + Border + 2, Location.Y + Border + 2), Foreground);
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        }
        public override void Reload()
        {
            Softwrapped = Font != null ? LimitLines(SoftwrapWords(Text, Font, Inside), Font, Inside.Y) : Text;
            Inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
        }
    }
}
