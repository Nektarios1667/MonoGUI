namespace MonoGUI
{
    public class HorizontalSlider : Widget
    {
        public event Action<float> ValueChanged;
        public int Length { get; private set; }
        public Color Color { get; private set; }
        public Color Highlight { get; private set; }
        public Color Background { get; private set; }
        public int Thickness { get; private set; }
        public int Size { get; private set; }
        private int State { get; set; }
        public float Value { get; set; }
        // Private
        private bool dragging = false;
        private MouseState previousState;
        public HorizontalSlider(GUI gui, Point location, int length, Color color, Color highlight, Color? background = null, int thickness = 3, int size = 7) : base(gui, location)
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
            if (PointCircleCollide(Gui.MousePosition, new Vector2(Location.X + Value * Length, Location.Y), Size) || dragging)
            {
                // Clicking
                if (Gui.LMouseDown)
                {
                    // Update
                    State = 2;
                    float val = Math.Clamp((Gui.MousePosition.X - Location.X) / (float)Length, 0, 1);
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
            Gui.Batch.DrawLine(Location.ToVector2(), new(Location.X + Length, Location.Y), Background, Thickness);
            // Circle
            for (int i = Size - 1; i > 0; i--)
            {
                Gui.Batch.DrawCircle(new(new(Location.X + Value * Length, Location.Y), i), 20, State == 0 ? Color : Highlight);
            }
            // Outline
            Gui.Batch.Draw(Gui.CircleOutline, new(Location.X + Value * Length, Location.Y), null, State == 0 ? Color.Black : (State == 1 ? Color.Gray : Color.DarkGray), 0, new(50, 50), Size / 50f, SpriteEffects.None, 0f);

        }

        // When changed value
        private void OnValueChanged(float newValue)
        {
            ValueChanged?.Invoke(newValue); // Invoke the event if any listeners are attached
        }
    }
}
