using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGUI.Widgets;
public class Group : Widget
{
    public List<Widget> Widgets { get; private set; } = new List<Widget>();
    public Group(GUI gui) : base(gui, Point.Zero) { }
    public override void Update()
    {
        if (!Visible) return;
        foreach (var widget in Widgets) widget.Update();
    }
    public override void Draw()
    {
        if (!Visible) return;
        foreach (var widget in Widgets) widget.Draw();
    }
    public void AddWidget(Widget widget) { Widgets.Add(widget); }
    public void AddWidgets(params Widget[] widgets) { Widgets.AddRange(widgets); }
    public void RemoveWidget(Widget widget) { Widgets.Remove(widget); }
    public void RemoveWidget(int idx) { Widgets.RemoveAt(idx); }
}
