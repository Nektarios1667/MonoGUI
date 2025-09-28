namespace MonoGUI
{
    public class VerticalSlider : Widget
    {
        public event Action<float> ValueChanged;
        public int Length { get; private set; }
        public Color Color { get; private set; }
        public Color Highlight { get; private set; }
        public Color Background { get; private set; }
        public int Thickness { get; private set; }
        public int Size { get; private set; }
        public int State { get; private set; }
        public float Value { get; set; }
        // Private
        private bool dragging = false;
        public VerticalSlider(GUI gui, Point location, int length, Color color, Color highlight, Color? background = null, int thickness = 3, int size = 7) : base(gui, location)
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
            if (PointCircleCollide(Gui.MousePosition, new Vector2(Location.X, Location.Y + Value * Length), Size) || dragging)
            {
                // Clicking
                if (Gui.LMouseDown)
                {
                    // Update
                    State = 2;
                    float val = Math.Clamp((Gui.MousePosition.Y - Location.Y) / (float)Length, 0, 1);
                    dragging = true;

                    // Change
                    if (val != Value)
                    {
                        Value = val;
                        OnValueChanged(Value);
                    }
                }
                else { dragging = false; State = 1; }
            }
            else { dragging = false; State = 0; }
        }

        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Gui.CircleOutline == null) { return; }

            // Line
            Gui.Batch.DrawLine(Location.ToVector2(), new(Location.X, Location.Y + Length), Background, Thickness);
            // Circle
            for (int i = Size - 1; i > 0; i--)
            {
                Gui.Batch.DrawCircle(new(new(Location.X, Location.Y + Value * Length), i), (int)(Size * 1.5), State == 0 ? Color : Highlight);
            }
            // Outline
            Gui.Batch.Draw(Gui.CircleOutline, new(Location.X, Location.Y + Value * Length), Gui.CircleOutline.Bounds, Color.Black, 0, new(7, 7), Size / 7, SpriteEffects.None, 0f);
        }

        // When changed value
        private void OnValueChanged(float newValue)
        {
            ValueChanged?.Invoke(newValue); // Invoke the event if any listeners are attached
        }
    }
}
