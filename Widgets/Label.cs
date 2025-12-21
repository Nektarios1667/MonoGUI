namespace MonoGUI;

public class Label : Widget
{
    public string Text { get; set; }
    public Color Color { get; set; }
    public SpriteFont? Font { get; set; }
    // Centering
    public Label(GUI gui, Point location, Color color, string text, SpriteFont? font = default) : base(gui, location)
    {
        Text = text;
        Font = font == default ? gui.Font : font;
        Color = color;
    }
    public override void Update() { }
    public override void Draw()
    {
        // Not drawing
        if (!Visible) { return; }
        if (Font == null) { return; }

        // Text
        Gui.Batch.DrawString(Font, Text, Location.ToVector2(), Color);
    }
}
