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
    public class Checkbox : Widget
    {
        public int Size { get; private set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, Size, Size); }
        }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public Color Foreground { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public bool Checked { get; private set; }
        public int State { get; private set; }
        public int CheckThickness { get; private set; }
        private Xna.Vector2 CheckLocation { get; set; }
        private Xna.Vector2 CheckDimensions { get; set; }
        private bool Last { get; set; }
        // Centering
        private Xna.Vector2 offset { get; set; }
        public Checkbox(GUI gui, Xna.Vector2 location, int size, Color foreground, Xna.Color color, Xna.Color highlight, int border = 3, Color borderColor = default, int checkThickness = 4) : base(gui, location)
        {
            Size = size;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            Checked = false;
            CheckThickness = checkThickness;
            CheckLocation = new(Location.X + Size / 2f, Location.Y + Size / 2f);
            int checkSize = Size - CheckThickness - Border * 2;
            CheckDimensions = new(checkSize, checkSize);
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            MouseState mouseState = Gui.MouseState;
            bool pressed = mouseState.LeftButton == ButtonState.Pressed;
            if (PointRectCollide(Location, new(Size, Size), mouseState.Position))
            {
                // Clicking
                if (pressed && !Last)
                {
                    Checked = !Checked;
                    State = 2;
                }
                else { State = 1; }
            } else { State = 0; }
            Last = pressed;
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, State == 0 ? Color : Highlight);
            // Draw X
            if (Checked)
            {
                DrawX(Gui.Batch, CheckLocation, CheckDimensions, Foreground, thickness: CheckThickness);
            }
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        }
        public override void Reload()
        {
            // Check
            CheckLocation = new(Location.X + Size / 2f, Location.Y + Size / 2f);
            int checkSize = Size - CheckThickness - Border * 2;
            CheckDimensions = new(checkSize, checkSize);
        }
    }
}
