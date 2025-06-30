namespace MonoGUI
{
    public class Dropdown : Widget
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
        public bool Open { get; set; }
        public Button SelectButton { get; private set; }
        // private
        private int arrowSize;
        private MouseState previousState;
        private int itemHeight;
        public Dropdown(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, SpriteFont? font = default, int seperation = 2, int border = 2, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Items = [];
            Selected = "";
            Font = font == default ? gui.Arial : font;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Seperation = seperation;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            arrowSize = Math.Min(Dimensions.X, Dimensions.Y) / 2;
            SelectButton = new(Gui, location, dimensions, foreground, color, highlight, ToggleOpen, [], text:Selected, font: Font, border: Border, borderColor: BorderColor, shift: -arrowSize + 2);
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + 4 + Seperation * 2 : 0;
            Open = false;
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Update selection button
            SelectButton.Update();

            // Update items
            if (Open)
            {
                foreach (Button item in Items) { item.Update(); }
            }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null || Gui.ArrowDown == null) { return; }

            Rectangle rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);

            // Draw selection button
            SelectButton.Draw();

            // Draw items
            if (Open)
            {
                foreach (Button item in Items) { item.Draw(); }
            }

            // Draw arrows
            arrowSize = (int)Math.Min(Dimensions.X, Dimensions.Y) / 2;
            Vector2 arrowLocation = new(Location.X + Dimensions.X - Border / 2 - arrowSize, Location.Y + Dimensions.Y / 2);
            float scale = Math.Min(Dimensions.X, Dimensions.Y) / 40f;
            Gui.Batch.Draw(Gui.ArrowDown, arrowLocation, null, BorderColor, Open ? 0 : 0, new Vector2(8, 5), scale, SpriteEffects.None, 0f);
        }
        public override void Reload()
        {
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + 4 + Seperation * 2 : 0;
            arrowSize = (int)Math.Min(Dimensions.X, Dimensions.Y) / 2;
            // Selectbutton updates
            SelectButton = new(Gui, Location, Dimensions, Foreground, Color, Highlight, ToggleOpen, [], text:Selected, font: Font, border: Border, borderColor: BorderColor, shift: -arrowSize + 2);
        }
        public void AddItems(params string[] texts)
        {
            foreach (string text in texts)
            {
                Point loc = new(Location.X + Border, Location.Y + Dimensions.Y - Seperation + itemHeight * Items.Count);
                Point dim = new(Dimensions.X - Border - Seperation, itemHeight);
                Items.Add(new(Gui, loc, dim, Foreground, Color, Highlight, SelectItem, [text], text, Font, border: Seperation, borderColor: BorderColor));
            }
        }
        public void ToggleOpen() { Open = !Open; }
        public void SelectItem(string item) { Selected = item; SelectButton.Modify("Text", LimitString(item, Font, Dimensions.X - Border - arrowSize * 2)); OnItemSelected(item); Open = false; }
        // When changed value
        public virtual void OnItemSelected(string item)
        {
            ItemSelected?.Invoke(item); // Invoke the event if any listeners are attached
        }
    }
}
