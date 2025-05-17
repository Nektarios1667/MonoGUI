using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Xna = Microsoft.Xna.Framework;

namespace MonoGUI
{
    public class VerticalSlider : Widget
    {
        public event Action<float> ValueChanged;
        public int Length { get; private set; }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public Color Background { get; private set; }
        public int Thickness { get; private set; }
        public int Size { get; private set; }
        public int State { get; private set; }
        public float Value { get; private set; }
        // Private
        private bool dragging = false;
        private MouseState previousState;
        private Xna.Vector2 previousCirclePosition;
        public VerticalSlider(GUI gui, Xna.Vector2 location, int length, Color color, Color highlight, Color? background = null, int thickness = 3, int size = 7) : base(gui, location)
        {
            Length = length;
            Color = color;
            Highlight = highlight;
            Background = background ?? Color.DarkGray;
            Thickness = thickness;
            Size = size;
            Value = 0;
            State = 0; // 0 = nothing, 1 = hover, 2 = click
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            MouseState mouseState = Gui.MouseState;
            if (PointCircleCollide(mouseState.Position, new Xna.Vector2(Location.X, Location.Y + Value * Length), Size) || dragging)
            {
                // Clicking
                if (mouseState.LeftButton == ButtonState.Pressed && (previousState.LeftButton != ButtonState.Pressed || dragging))
                {
                    // Update
                    State = 2;
                    float val = Math.Clamp((mouseState.Position.Y - Location.Y) / Length, 0, 1);
                    dragging = true;

                    // Change
                    if (val != Value)
                    {
                        Value = val;
                        OnValueChanged(Value);
                    }
                    previousCirclePosition = mouseState.Position.ToVector2();

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
            if (Gui.CircleOutline == null) { return; }

            // Line
            Gui.Batch.DrawLine(Location, new(Location.X, Location.Y + Length), Background, Thickness);
            // Circle
            for (int i = Size - 1; i > 0; i--)
            {
                Gui.Batch.DrawCircle(new(new(Location.X, Location.Y + Value * Length), i), (int)(Size * 1.5), State == 0 ? Color : Highlight);
            }
            // Outline
            Gui.Batch.Draw(Gui.CircleOutline, new(Location.X, Location.Y + Value * Length), Gui.CircleOutline.Bounds, Color.Black, 0, new(7, 7), Size / 7, SpriteEffects.None, 0f);
        }

        // When changed value
        public virtual void OnValueChanged(float newValue)
        {
            ValueChanged?.Invoke(newValue); // Invoke the event if any listeners are attached
        }
    }
}
