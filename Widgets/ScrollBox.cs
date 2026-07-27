namespace MonoGUI;

/// <summary>A selectable list with a viewport and a proportional vertical scrollbar.</summary>
public class ScrollBox : Widget, ILinkable
{
    private readonly ScrollBar _scrollBar;
    private int _itemHeight;

    public string LinkableValue => _scrollBar.LinkableValue;
    public event Action<string>? ItemSelected;
    public Point Dimensions { get; set; }
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public SpriteFont? Font { get; set; }
    public Color Foreground { get; set; }
    public int Seperation { get; set; }
    public int Border { get; set; }
    public List<Button> Items { get; } = [];
    public string Selected { get; private set; } = string.Empty;
    public TextAlign Align { get; set; }
    public ScrollBar ScrollBar => _scrollBar;

    public ScrollBox(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight,
        TextAlign align = TextAlign.Left, SpriteFont? font = default, int seperation = 1, int border = 3,
        Color borderColor = default, Color? thumbColor = null, Color? thumbHighlight = null, Color? troughColor = null) : base(gui, location)
    {
        Dimensions = dimensions;
        Font = font ?? gui.Font;
        Align = align;
        Foreground = foreground;
        Color = color;
        Highlight = highlight;
        Seperation = Math.Max(0, seperation);
        Border = Math.Max(0, border);
        BorderColor = borderColor == default ? Microsoft.Xna.Framework.Color.Black : borderColor;
        _itemHeight = Font is null ? 0 : Font.LineSpacing + Seperation * 2;
        _scrollBar = new ScrollBar(gui, Point.Zero, 1, thumbColor ?? Microsoft.Xna.Framework.Color.Black,
            thumbHighlight ?? GUI.NearBlack, background: troughColor ?? Microsoft.Xna.Framework.Color.Gray);
        Layout();
    }

    public Color BorderColor { get; set; }

    public override void Update()
    {
        if (!Visible || !Enabled) return;
        Layout();
        _scrollBar.Update();

        int offset = ScrollOffset;
        for (int index = 0; index < Items.Count; index++)
        {
            Button item = Items[index];
            item.Location = new Point(Location.X + Border, ViewportTop + index * _itemHeight - offset);
            if (IsFullyInViewport(item.Rect)) item.Update();
        }
    }

    public override void Draw()
    {
        if (!Visible) return;
        Rectangle rect = new(Location, Dimensions);
        Gui.Batch.FillRectangle(rect, Color);
        Gui.Batch.DrawRectangle(rect, BorderColor, Border);
        if (Font is null) return;

        foreach (Button item in Items)
        {
            if (!IsFullyInViewport(item.Rect)) continue;
            item.Draw();
            if (Selected == item.Text)
            {
                Rectangle selected = item.Rect;
                selected.Inflate(-Math.Max(1, item.Border), -Math.Max(1, item.Border));
                Gui.Batch.FillRectangle(selected, Highlight * 0.5f);
            }
        }
        _scrollBar.Draw();
    }

    public override void Reload()
    {
        _itemHeight = Font is null ? 0 : Font.LineSpacing + Seperation * 2;
        foreach (Button item in Items) item.Font = Font;
        Layout();
    }

    public void AddItems(params string[] texts)
    {
        ArgumentNullException.ThrowIfNull(texts);
        foreach (string text in texts)
        {
            Button item = new(Gui, Point.Zero, new Point(Math.Max(1, Dimensions.X - Border * 2), Math.Max(1, _itemHeight)),
                Foreground, Color, Highlight, SelectItem, [text], text, align: Align, font: Font, border: Seperation, borderColor: BorderColor);
            Items.Add(item);
        }
        Layout();
    }

    public void ClearItems()
    {
        Items.Clear();
        Selected = string.Empty;
        _scrollBar.SetValue(0, notify: false);
        Layout();
    }

    public void SelectItem(string item)
    {
        if (Selected == item) return;
        Selected = item;
        ItemSelected?.Invoke(item);
    }

    private int ViewportTop => Location.Y + Border;
    private int ViewportHeight => Math.Max(0, Dimensions.Y - Border * 2);
    private int ContentHeight => Items.Count * _itemHeight;
    private int ScrollOffset => (int)MathF.Round(_scrollBar.Value * Math.Max(0, ContentHeight - ViewportHeight));

    private void Layout()
    {
        bool needsScrollbar = ContentHeight > ViewportHeight;
        _scrollBar.Location = new Point(Location.X + Dimensions.X + Border + 5, ViewportTop);
        _scrollBar.Length = ViewportHeight;
        _scrollBar.BarSize = needsScrollbar && ContentHeight > 0
            ? Math.Clamp((int)MathF.Round(ViewportHeight * (ViewportHeight / (float)ContentHeight)), 12, Math.Max(12, ViewportHeight))
            : Math.Max(1, ViewportHeight);
        _scrollBar.Visible = needsScrollbar;
        if (!needsScrollbar) _scrollBar.SetValue(0, notify: false);
    }

    private bool IsFullyInViewport(Rectangle rect) => rect.Top >= ViewportTop && rect.Bottom <= ViewportTop + ViewportHeight && rect.Left >= Location.X + Border && rect.Right <= Location.X + Dimensions.X - Border;
}
