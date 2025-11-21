namespace MonoGUI
{
    public class ScrollBox : Widget
    {
        public event Action<string> ItemSelected;
        public Point Dimensions { get; private set; }
        public Color Color { get; private set; }
        public Color Highlight { get; private set; }
        public SpriteFont? Font { get; private set; }
        public Color Foreground { get; private set; }
        public int Seperation { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public List<Button> Items { get; private set; }
        public string Selected { get; private set; }
        public TextAlign Align { get; private set; }
        // Private
        private VerticalSlider ScrollBar;
        private int itemHeight;
        public ScrollBox(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, TextAlign align = TextAlign.Left, SpriteFont? font = default, int seperation = 1, int border = 3, Color borderColor = default, Color? thumbColor = null, Color? thumbHighlight = null, Color? troughColor = null) : base(gui, location)
        {
            Dimensions = dimensions;
            Items = [];
            Selected = "";
            Font = font == default ? gui.Font : font;
            Align = align;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Seperation = seperation;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
            ScrollBar = new(gui, new(location.X + dimensions.X + border + 5, location.Y + border), dimensions.Y - border * 2, color: thumbColor ?? Color.Black, highlight: thumbHighlight ?? GUI.NearBlack, background: troughColor ?? Color.Gray);
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Scrollbar
            if (Items.Count * itemHeight > Dimensions.Y - Border * 2)
                ScrollBar.Visible = true;
            else
            {
                ScrollBar.Visible = false;
                ScrollBar.Value = 0;
            }
            ScrollBar.Update();

            // Hovering
            for (int b = 0; b < Items.Count; b++)
            {
                // Item
                Button item = Items[b];
                // Location
                // default location                              scrolling             extra line                 remove one page
                item.Location = new(item.Location.X, Location.Y + Border - Seperation + (itemHeight * b) - (int)(ScrollBar.Value * itemHeight * (Items.Count + 1)) + (int)(ScrollBar.Value * Dimensions.Y));
                // Check X
                if (item.Location.X < Location.X || item.Location.X + item.Dimensions.X > Location.X + Dimensions.X) { continue; }
                // Check Y
                if (item.Location.Y < Location.Y || item.Location.Y + item.Dimensions.Y > Location.Y + Dimensions.Y) { continue; }
                // Update
                item.Update();
            }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null) { return; }

            Rectangle rect = new(Location, Dimensions);
            // Background
            Gui.Batch.FillRectangle(rect, Color);
            // Outline
            Gui.Batch.DrawRectangle(rect, BorderColor, Border);

            // Items
            foreach (Button item in Items)
            {
                // Check X
                if (item.Location.X < Location.X || item.Location.X + item.Dimensions.X > Location.X + Dimensions.X) { continue; }
                // Check Y
                if (item.Location.Y < Location.Y || item.Location.Y + item.Dimensions.Y > Location.Y + Dimensions.Y) { continue; }
                // Draw
                item.Draw();
                if (Selected == item.Text)
                {
                    Rectangle highlightRect = new(item.Location.X + Border + 1, item.Location.Y + Border + 1, item.Dimensions.X - Border * 2 - 2, item.Dimensions.Y - Border * 2 - 2);
                    Gui.Batch.FillRectangle(highlightRect, Highlight * 0.5f);
                }
            }

            // Scrollbar
            ScrollBar.Draw();
        }
        public override void Reload()
        {
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
        }
        public void AddItems(params string[] texts)
        {
            foreach (string text in texts)
            {
                Point loc = new(Location.X + Border - Seperation, Location.Y + Border / 2 - Seperation + itemHeight * Items.Count);
                Point dim = new(Dimensions.X - Border - Seperation, itemHeight);
                Items.Add(new(Gui, loc, dim, Foreground, Color, Highlight, SelectItem, [text], text, align: Align, font: Font, border: Seperation, borderColor: BorderColor));
            }
        }
        public void SelectItem(string item) { Selected = item; OnItemSelected(item); }
        // When changed value
        public virtual void OnItemSelected(string item)
        {
            ItemSelected?.Invoke(item); // Invoke the event if any listeners are attached
        }
    }
}
