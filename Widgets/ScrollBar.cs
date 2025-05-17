using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Xna = Microsoft.Xna.Framework;

namespace MonoGUI
{
    public class ScrollBar : Widget
    {
        public event Action<float> ValueChanged;
        public int Length { get; private set; }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public Color Background { get; private set; }
        public int Width { get; private set; }
        public int State { get; private set; }
        public float Value { get; private set; }
        public int BarSize { get; private set; }
        // Private
        private bool dragging = false;
        private MouseState previousState;
        public ScrollBar(GUI gui, Xna.Vector2 location, int length, Color color, Color highlight, int barSize = 20, Color? background = null, int width = 10, int size = 7) : base(gui, location)
        {
            Length = length;
            Color = color;
            Highlight = highlight;
            BarSize = barSize;
            Background = background ?? Color.DarkGray;
            Width = width;
            Value = 0;
            State = 0; // 0 = nothing, 1 = hover, 2 = click
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            MouseState mouseState = Gui.MouseState;
            if (PointRectCollide(new((int)Location.X, (int)(Location.Y + (Value * Length)), Width, Length), mouseState.Position) || dragging)
            {
                // Clicking
                if (mouseState.LeftButton == ButtonState.Pressed && (previousState.LeftButton != ButtonState.Pressed || dragging))
                {
                    // Update
                    State = 2;
                    float move = (mouseState.Position.Y - previousState.Position.Y) / (float)Length;
                    dragging = true;

                    // Change
                    if (move != 0)
                    {
                        Value = Math.Clamp((Value + move), 0f, 1f);
                        OnValueChanged(Value);
                    }
                }
                else { dragging = false; State = 1; }
            }
            else { dragging = false; State = 0; }

            // previousState
            previousState = mouseState;
        }

        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(new(Location.X, Location.Y, Width, Length), Background);
            // Square
            Gui.Batch.FillRectangle(new(Location.X, Location.Y + (Value * Length), Width, BarSize), Color);
        }

        // When changed value
        public virtual void OnValueChanged(float newValue)
        {
            ValueChanged?.Invoke(newValue); // Invoke the event if any listeners are attached
        }
    }
}
