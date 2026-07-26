using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGUI.Widgets;

public class Rectangle : Widget
{
    public Microsoft.Xna.Framework.Rectangle Rect { get; set; }
    public Color FillColor { get; set; }
    public Color BorderColor { get; set; }
    public int BorderThickness { get; set; }
    public Rectangle(GUI gui, Point location, Point size, Color fill, Color border, int borderThickness) : base(gui, location)
    {
        Rect = new(location, size);
        FillColor = fill;
        BorderColor = border;
        BorderThickness = borderThickness;
    }
    public override void Update() { }
    public override void Draw()
    {
        Gui.Batch.FillRectangle(Rect, FillColor);
        Gui.Batch.DrawRectangle(Rect, BorderColor, BorderThickness);
    }
}
