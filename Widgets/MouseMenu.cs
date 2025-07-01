using System.Linq;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace MonoGUI;

public class MouseMenu : Widget
{
    public Point Dimensions { get; private set; }
    public Color Color { get; private set; }
    public Color Highlight { get; private set; }
    public SpriteFont? Font { get; private set; }
    public Color Foreground { get; private set; }
    public int Seperation { get; private set; }
    public int Border { get; private set; }
    public Color BorderColor { get; private set; }
    public List<MenuItem> MenuItems { get; private set; }
    public bool Root { get; set; }
    // Private
    private Point lastLocation { get; set; }
    private int itemHeight;
    public MouseMenu(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, bool root = true, SpriteFont? font = default, int seperation = 1, int border = 3, Color borderColor = default) : base(gui, location)
    {
        Dimensions = dimensions;
        MenuItems = [];
        Font = font == default ? gui.Font : font;
        Foreground = foreground;
        Color = color;
        Highlight = highlight;
        Seperation = seperation;
        Border = border;
        BorderColor = (borderColor == default ? Color.Black : borderColor);
        Root = root;
        Visible = false;
        lastLocation = Location;
        itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
    }
    public override void Update()
    {
        // Open
        if (Root && Gui.RMouseClicked && (!PointRectCollide(Location, Dimensions, Gui.MousePosition) || !Visible))
        {
            Visible = true;
            Location = Gui.MousePosition;
        }

        // Close
        if ((Gui.LMouseClicked || Gui.MMouseClicked || Gui.RMouseClicked) && !MouseInMenu())
            CloseMenu();

        // Visible
        if (!Visible) return;

        // Update Items
        int y = Border;
        foreach (MenuItem item in MenuItems)
        {
            // Update button first
            Button button = item.Button;
            button.Location = new Point(Location.X + Border, Location.Y + y);
            button.Update();

            // Update submenu
            if (item.SubMenu != null)
            {
                item.SubMenu.Location = new Point(Location.X + Dimensions.X - Border, button.Location.Y);
                item.SubMenu.Update();
                if (button.State >= 1)
                    item.SubMenu.Visible = true;
                else if (!item.SubMenu.MouseInMenu())
                    item.SubMenu.Visible = false;
            }

            // Update y
            y += itemHeight + Seperation;
        }
    }
    public override void Draw()
    {
        // Not drawing
        if (!Visible) { return; }
        if (Font == null) { return; }

        Rectangle rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
        // Background
        Gui.Batch.FillRectangle(rect, Color);
        // Outline
        Gui.Batch.DrawRectangle(rect, BorderColor, Border);

        // Items
        foreach (MenuItem item in MenuItems)
        {
            item.SubMenu?.Draw();
            item.Button.Draw();
        }
    }
    public override void Reload()
    {
        itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
    }
    public bool MouseInMenu()
    {
        if (PointRectCollide(Location, Dimensions, Gui.MousePosition)) return true;
        foreach (MenuItem item in MenuItems)
            if (item.SubMenu?.MouseInMenu() == true) return true;
        return false;
    }
    public void CloseMenu()
    {
        Visible = false;
        foreach (MenuItem item in MenuItems) item.CloseSubMenu();
    }
    private void RunMenuItem(Delegate? action, object?[] args)
    {
        action?.DynamicInvoke(args);
        Visible = false; // Close menu after action
    }
    public void AddItem(string text, Delegate? action, object?[] args)
    {
        Point dim = new(Dimensions.X - Border * 2, itemHeight);
        Button button = new(Gui, Point.Zero, dim, Foreground, Color, Highlight, RunMenuItem, [action, args], text, Font, border: Seperation, borderColor: Color);
        MenuItems.Add(new(button));
    }
    public void AddSubMenu(string button, MouseMenu subMenu)
    {
        MenuItem item = MenuItems.FirstOrDefault(i => i.Button.Text == button) ?? throw new ArgumentException($"No item with text '{button}' found.");
        item.AttachSubMenu(subMenu);
    }
}
