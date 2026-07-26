using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGUI.Widgets;

public class RectangleShape : Widget
{
    public Point Size { get; set; }
    public Color FillColor { get; set; }
    public Color BorderColor { get; set; }
    public int BorderThickness { get; set; }
    public RectangleShape(GUI gui, Point location, Point size, Color fill, Color border, int borderThickness) : base(gui, location)
    {
        Size = size;
        FillColor = fill;
        BorderColor = border;
        BorderThickness = borderThickness;
    }
    public override void Update() { }
    public override void Draw()
    {
        Gui.Batch.FillRectangle(new(Location.ToVector2(), Size), FillColor);
        Gui.Batch.DrawRectangle(new(Location.ToVector2(), Size), BorderColor, BorderThickness);
    }
}
