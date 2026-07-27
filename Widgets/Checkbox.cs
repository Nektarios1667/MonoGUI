namespace MonoGUI;

public class Checkbox : Widget, ILinkable
{
    public string LinkableValue => Checked.ToString();
    public int Size { get; set; }
    public Rectangle Rect
    {
        get { return new(Location.X, Location.Y, Size, Size); }
    }
    public event Action<bool>? ValueChanged;
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public Color Foreground { get; set; }
    public int Border { get; set; }
    public Color BorderColor { get; set; }
    public bool Checked { get; private set; }
    public int State { get; set; }
    public int CheckThickness { get; set; }
    private Point CheckLocation { get; set; }
    private Point CheckDimensions { get; set; }
    public Checkbox(GUI gui, Point location, int size, Color foreground, Color color, Color highlight, int border = 3, Color borderColor = default, int checkThickness = 4) : base(gui, location)
    {
        Size = size;
        Foreground = foreground;
        Color = color;
        Highlight = highlight;
        Border = border;
        BorderColor = (borderColor == default ? Color.Black : borderColor);
        Checked = false;
        CheckThickness = checkThickness;
        CheckLocation = new(Location.X + Size / 2, Location.Y + Size / 2);
        int checkSize = Size - CheckThickness - Border * 2;
        CheckDimensions = new(checkSize, checkSize);
    }
    public override void Update()
    {
        // Hidden
        if (!Visible || !Enabled) { State = 0; return; }

        // Hovering
        if (Rect.Contains(Gui.MousePosition))
        {
            // Clicking
            if (Gui.LMouseClicked)
            {
                Checked = !Checked;
                ValueChanged?.Invoke(Checked);
                State = 2;
            }
            else { State = 1; }
        }
        else { State = 0; }
    }
    public override void Draw()
    {
        // Not drawing
        if (!Visible) { return; }

        // Background
        Gui.Batch.FillRectangle(Rect, State == 0 ? Color : Highlight);
        // Draw X
        if (Checked)
        {
            DrawX(Gui.Batch, CheckLocation, CheckDimensions, Foreground, thickness: CheckThickness);
        }
        // Outline
        Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
    }
    public override void Reload()
    {
        // Check
        CheckLocation = new(Location.X + Size / 2, Location.Y + Size / 2);
        int checkSize = Size - CheckThickness - Border * 2;
        CheckDimensions = new(checkSize, checkSize);
    }
    public void Check() => SetValue(true);

    public void Uncheck() => SetValue(false);

    public void Toggle() => SetValue(!Checked);
    public void SetValue(bool value)
    {
        if (Checked == value) return;
        Checked = value;
        ValueChanged?.Invoke(Checked);
    }
}
