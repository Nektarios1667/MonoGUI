namespace MonoGUI
{
    public class ListBox : Widget
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
        public TextAlign Align { get; private set; }
        public string Selected { get; private set; }
        // Private
        private int itemHeight;
        public ListBox(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, SpriteFont? font = null, TextAlign align = TextAlign.Left, int seperation = 1, int border = 3, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Items = [];
            Selected = "";
            Font = font ?? gui.Font;
            Align = align;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Seperation = seperation;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            foreach (Button item in Items)
            {
                // Check X
                if (item.Location.X < Location.X || item.Location.X + item.Dimensions.X + item.Border > Location.X + Dimensions.X) { continue; }
                // Check Y
                if (item.Location.Y < Location.Y || item.Location.Y + item.Dimensions.Y + item.Border > Location.Y + Dimensions.Y) { continue; }
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
                if (item.Location.X < Location.X || item.Location.X + item.Dimensions.X + item.Border > Location.X + Dimensions.X) { continue; }
                // Check Y
                if (item.Location.Y < Location.Y || item.Location.Y + item.Dimensions.Y + item.Border > Location.Y + Dimensions.Y) { continue; }
                // Draw
                item.Draw();
                if (Selected == item.Text)
                {
                    Rectangle highlightRect = new(item.Location.X + Border + 1, item.Location.Y + Border + 1, item.Dimensions.X - Border * 2 - 2, item.Dimensions.Y - Border * 2 - 2);
                    Gui.Batch.FillRectangle(highlightRect, Highlight * 0.5f);
                }
            }
        }
        public override void Reload()
        {
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + Seperation * 2 : 0;
        }
        public void AddItems(params string[] texts)
        {
            foreach (string text in texts)
            {
                Point loc = new(Location.X + Border, Location.Y + Border + itemHeight * Items.Count);
                Point dim = new(Dimensions.X - Border - Seperation, itemHeight);
                Items.Add(new(Gui, loc, dim, Foreground, Color, Highlight, SelectItem, [text], text, font: Font, align: Align, border: Seperation, borderColor: BorderColor));
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
