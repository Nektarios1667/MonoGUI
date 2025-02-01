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

namespace MonoGUI
{
    public class Infobox : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        }
        public Xna.Color Color { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        private string _text { get; set; }
        public string Text {
            get { return _text; }
            set {
                _text = value;
                Softwrapped = SoftwrapWords(value, Font, Inside);
                Inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            }
        }
        public SpriteFont Font { get; set; }
        public Color Foreground { get; set; }
        public Xna.Rectangle Activation { get; set; }
        public string Softwrapped { get; set; }
        public Xna.Vector2 Inside { get; set; }
        public float Delay { get; set; }
        private float Time { get; set; }
        // Centering
        public Infobox(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Xna.Rectangle activation, Color color, Color foreground, string text, SpriteFont font, float delay = 1, int border = 2, Color? borderColor = null) : base(gui, location)
        {
            Dimensions = dimensions;
            Activation = activation;
            Color = color;
            Border = border;
            BorderColor = (borderColor == null ? Color.Black : (Color)borderColor);
            Visible = true;
            _text = text;
            Font = font;
            Foreground = foreground;
            Inside = new(Dimensions.X - Border * 2 - 4, Dimensions.Y - Border * 2 - 4);
            Softwrapped = SoftwrapWords(text, font, Inside);
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

        // Static
        public static string Softwrap(string text, SpriteFont font, Xna.Vector2 dimensions)
        {
            // setup
            string wrapped = "";
            int start = 0;
            int end = 1;

            while (end < text.Length)
            {
                // Wrap
                if (font.MeasureString(text[start..end]).X + 2 > dimensions.X) { wrapped += $"{text[start..end]}\n"; start = end; }
                end++;
            }
            return wrapped + text[start..end];
        }
        public static string SoftwrapWords(string text, SpriteFont font, Xna.Vector2 dimensions)
        {
            // setup
            string wrapped = "";
            int start = 0;
            int end = 1;

            while (end < text.Length)
            {
                // Wrap
                if (font.MeasureString(text[start..end]).X + 2 > dimensions.X) {
                    int cutoff = text[start..end].LastIndexOf(' ') + start;
                    if (cutoff <= start) { cutoff = end; }
                    wrapped += $"{text[start..cutoff]}\n";
                    start = cutoff + 1;
                    end = cutoff + 2;
                }
                end++;
            }
            wrapped += text[start..];
            return wrapped;
        }
    }
}
