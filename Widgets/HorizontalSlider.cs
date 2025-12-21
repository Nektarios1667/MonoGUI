namespace MonoGUI;

public class HorizontalSlider : Widget
{
    public event Action<float>? ValueChanged;
    public int Length { get; set; }
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public Color Background { get; set; }
    public int Thickness { get; set; }
    public int Size { get; set; }
    private int State { get; set; }
    public float Value { get; set; }
    // Private
    private bool dragging = false;
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
        Gui.Batch.Draw(Gui.CircleOutline, new(Location.X + Value * Length, Location.Y), null, State == 0 ? Color : Highlight, 0, new(12.5f, 12.5f), Size / 12.5f, SpriteEffects.None, 0f);

    }

    // When changed value
    private void OnValueChanged(float newValue)
    {
        ValueChanged?.Invoke(newValue); // Invoke the event if any listeners are attached
    }
}
