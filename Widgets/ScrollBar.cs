namespace MonoGUI;

/// <summary>A vertical scrollbar whose value represents the position of its thumb on the track.</summary>
public class ScrollBar : Widget, ILinkable
{
    private bool _dragging;
    private int _dragOffset;

    public string LinkableValue => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public event Action<float>? ValueChanged;
    public int Length { get; set; }
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public Color Background { get; set; }
    public int Width { get; set; }
    public int State { get; private set; }
    public float Value { get; private set; }
    public int BarSize { get; set; }
    public Rectangle TrackRect => new(Location.X, Location.Y, Width, Math.Max(0, Length));
    public Rectangle ThumbRect => new(Location.X, Location.Y + ThumbOffset, Width, EffectiveBarSize);

    public ScrollBar(GUI gui, Point location, int length, Color color, Color highlight, int barSize = 20,
        Color? background = null, int width = 10, int size = 7) : base(gui, location)
    {
        Length = Math.Max(0, length);
        Color = color;
        Highlight = highlight;
        BarSize = Math.Max(1, barSize);
        Background = background ?? Microsoft.Xna.Framework.Color.DarkGray;
        Width = Math.Max(1, width);
    }

    public override void Update()
    {
        if (!Visible || !Enabled) return;
        Point mouse = Gui.MousePosition;
        if (Gui.LMouseClicked && TrackRect.Contains(mouse))
        {
            _dragging = true;
            _dragOffset = ThumbRect.Contains(mouse) ? mouse.Y - ThumbRect.Y : EffectiveBarSize / 2;
            SetValueFromMouse(mouse.Y - _dragOffset);
        }
        if (_dragging && Gui.LMouseDown) SetValueFromMouse(mouse.Y - _dragOffset);
        if (_dragging && Gui.LMouseReleased) _dragging = false;
        State = _dragging ? 2 : ThumbRect.Contains(mouse) ? 1 : 0;
    }

    public override void Draw()
    {
        if (!Visible) return;
        Gui.Batch.FillRectangle(TrackRect, Background);
        Gui.Batch.FillRectangle(ThumbRect, State == 0 ? Color : Highlight);
    }

    public void SetValue(float newValue, bool notify = true)
    {
        float clamped = Math.Clamp(newValue, 0f, 1f);
        if (MathF.Abs(Value - clamped) < float.Epsilon) return;
        Value = clamped;
        if (notify) OnValueChanged(Value);
    }

    public virtual void OnValueChanged(float newValue) => ValueChanged?.Invoke(newValue);

    private int EffectiveBarSize => Math.Clamp(BarSize, 1, Math.Max(1, Length));
    private int Travel => Math.Max(0, Length - EffectiveBarSize);
    private int ThumbOffset => (int)MathF.Round(Value * Travel);
    private void SetValueFromMouse(int thumbY) => SetValue(Travel == 0 ? 0f : (thumbY - Location.Y) / (float)Travel);
}
