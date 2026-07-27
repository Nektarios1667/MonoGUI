using static System.Net.Mime.MediaTypeNames;

namespace MonoGUI;

public class LinkedLabel : Widget
{
    public string Prepend { get; set; }
    public string Append { get; set; }
    public Color Color { get; set; }
    public SpriteFont? Font { get; set; }
    public ILinkable? Link { get; set; }
    // Centering
    public LinkedLabel(GUI gui, Point location, Color color, ILinkable link, string prepend, string append, SpriteFont? font = default) : base(gui, location)
    {
        Link = link;
        Prepend = prepend;
        Append = append;
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
        Gui.Batch.DrawString(Font, $"{Prepend}{Link?.LinkableValue ?? string.Empty}{Append}", Location.ToVector2(), Color);
    }
}
