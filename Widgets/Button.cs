using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Xna = Microsoft.Xna.Framework;

namespace MonoGUI
{
    public class Button : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public string Text { get; private set; }
        private string CutoffText { get; set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public SpriteFont? Font { get; private set; }
        public Color Foreground { get; private set; }
        public Delegate? Function { get; set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public object?[]? Args { get; set; }
        public int State { get; private set; }
        public int Shift { get; private set; }
        // Private
        private Xna.Vector2 offset;
        private MouseState previousState;
        public Button(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Color foreground, Xna.Color color, Xna.Color highlight, Delegate? function, string text = "", SpriteFont? font = default, object?[]? args = null, int border = 3, Color borderColor = default, int shift = 0) : base(gui, location)
        {
            Dimensions = dimensions;
            Text = text;
            Font = font == default ? gui.Arial : font;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Function = function;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            Args = args;
            State = 0;
            Shift = shift;

            Xna.Vector2 inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            CutoffText = Font != null ? LimitString(text, Font, inside.X - 2) : Text;
            Xna.Vector2 textDim = Font != null ? Font.MeasureString(CutoffText) : new(0, 0);
            offset = Xna.Vector2.Floor((inside - textDim) / 2);
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            MouseState mouseState = Gui.MouseState;
            bool pressed = mouseState.LeftButton == ButtonState.Pressed;
            if (PointRectCollide(Location, new(Dimensions.X - Border, Dimensions.Y - Border), mouseState.Position))
            {
                // Clicking
                if (pressed && previousState.LeftButton != ButtonState.Pressed)
                {
                    State = 2;
                    Function?.DynamicInvoke(Args);
                }
                else { State = 1; }
            }
            else { State = 0; }
            previousState = mouseState;
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, State == 0 ? Color : Highlight);
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);

            // Text
            if (Font != null)
            {
                Gui.Batch.DrawString(Font, CutoffText, new(Location.X + Border + offset.X + Shift, Location.Y + Border + offset.Y), Foreground);
            }
            else if (Text != "")
            {
                Console.WriteLine($"Skipping drawing text '{Text}' because of uninitialized font");
            }
        }
        public override void Reload()
        {
            // Text dim
            Xna.Vector2 textDim = Font != null ? Font.MeasureString(Text) : new(0, 0);
            Xna.Vector2 inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            offset = Xna.Vector2.Floor((inside - textDim) / 2);
            // Cutoff text
            CutoffText = Font != null ? LimitString(Text, Font, inside.X) : Text;
        }
    }
}
