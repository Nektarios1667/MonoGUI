namespace MonoGUI
{
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
        public List<Button> Items { get; private set; }
        // Private
        private int itemHeight;
        public MouseMenu(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, SpriteFont? font = default, int seperation = 1, int border = 3, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Items = [];
            Font = font == default ? gui.Font : font;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Seperation = seperation;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            Visible = false;
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
        }
        public override void Update()
        {
            // Open
            if (Gui.RMouseClicked)
            {
                // Check if mouse is inside the menu
                if (!PointRectCollide(Location, Dimensions, Gui.MousePosition) || !Visible)
                {
                    Visible = true;
                    foreach (Button item in Items) item.Location += Gui.MousePosition - Location;
                    Location = Gui.MousePosition;
                }
            }
            // Close
            if ((Gui.LMouseClicked || Gui.MMouseClicked || Gui.RMouseClicked) && !PointRectCollide(Location, Dimensions, Gui.MousePosition))
                Visible = false;


            // Hovering
            foreach (Button item in Items) item.Update();
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
            foreach (Button item in Items) item.Draw();
        }
        public override void Reload()
        {
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
        }
        private void RunMenuItem(Delegate? action, object?[] args)
        {
            action?.DynamicInvoke(args);
            Visible = false; // Close menu after action
        }
        public void AddItem(string text, Delegate? action, object?[] args)
        {
            Point loc = new(Location.X + Border, Location.Y + Border + (itemHeight + Seperation) * Items.Count);
            Point dim = new(Dimensions.X - Border - Seperation, itemHeight);
            Items.Add(new(Gui, loc, dim, Foreground, Color, Highlight, RunMenuItem, [action, args], text, Font, border:0, borderColor: Color));
        }
    }
}
