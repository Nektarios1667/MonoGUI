namespace MonoGUI;

public enum ProgressBarDirection
{
    LeftToRight,
    RightToLeft,
    BottomToTop,
    TopToBottom,
}

/// <summary>Displays progress between zero and one, optionally with a percentage label.</summary>
public class ProgressBar : Widget, ILinkable
{
    private float _value;

    public string LinkableValue => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public Point Dimensions { get; set; }
    public Rectangle Bounds => new(Location, Dimensions);
    public float Value => _value;
    public Color BackgroundColor { get; set; }
    public Color FillColor { get; set; }
    public Color BorderColor { get; set; }
    public int Border { get; set; }
    public bool ShowPercentage { get; set; }
    public SpriteFont? Font { get; set; }
    public Color TextColor { get; set; }
    public ProgressBarDirection Direction { get; set; }
    public event Action<float>? ValueChanged;

    public ProgressBar(GUI gui, Point location, Point dimensions, Color backgroundColor, Color fillColor,
        float value = 0f, int border = 2, Color borderColor = default, SpriteFont? font = null,
        Color textColor = default, bool showPercentage = false, ProgressBarDirection direction = ProgressBarDirection.LeftToRight)
        : base(gui, location)
    {
        if (dimensions.X <= 0 || dimensions.Y <= 0) throw new ArgumentOutOfRangeException(nameof(dimensions));
        Dimensions = dimensions;
        BackgroundColor = backgroundColor;
        FillColor = fillColor;
        Border = Math.Max(0, border);
        BorderColor = borderColor == default ? Microsoft.Xna.Framework.Color.Black : borderColor;
        Font = font ?? gui.Font;
        TextColor = textColor == default ? Microsoft.Xna.Framework.Color.Black : textColor;
        ShowPercentage = showPercentage;
        Direction = direction;
        SetValue(value, notify: false);
    }

    public override void Update() { }

    public override void Draw()
    {
        if (!Visible) return;
        Gui.Batch.FillRectangle(Bounds, BackgroundColor);
        Rectangle fill = FillBounds;
        if (fill.Width > 0 && fill.Height > 0) Gui.Batch.FillRectangle(fill, FillColor);
        if (Border > 0) Gui.Batch.DrawRectangle(Bounds, BorderColor, Border);

        if (!ShowPercentage || Font is null) return;
        string text = $"{MathF.Round(Value * 100):0}%";
        Vector2 textSize = Font.MeasureString(text);
        Gui.Batch.DrawString(Font, text, new Vector2(Location.X + (Dimensions.X - textSize.X) / 2f, Location.Y + (Dimensions.Y - textSize.Y) / 2f), TextColor);
    }

    public void SetValue(float value, bool notify = true)
    {
        float clamped = Math.Clamp(value, 0f, 1f);
        if (MathF.Abs(_value - clamped) < float.Epsilon) return;
        _value = clamped;
        if (notify) ValueChanged?.Invoke(Value);
    }

    public void Increment(float amount) => SetValue(Value + amount);
    public void Reset(bool notify = true) => SetValue(0f, notify);

    private Rectangle FillBounds
    {
        get
        {
            int inset = Border;
            int width = Math.Max(0, Dimensions.X - inset * 2);
            int height = Math.Max(0, Dimensions.Y - inset * 2);
            int fillWidth = (int)MathF.Round(width * Value);
            int fillHeight = (int)MathF.Round(height * Value);
            return Direction switch
            {
                ProgressBarDirection.RightToLeft => new Rectangle(Location.X + inset + width - fillWidth, Location.Y + inset, fillWidth, height),
                ProgressBarDirection.BottomToTop => new Rectangle(Location.X + inset, Location.Y + inset + height - fillHeight, width, fillHeight),
                ProgressBarDirection.TopToBottom => new Rectangle(Location.X + inset, Location.Y + inset, width, fillHeight),
                _ => new Rectangle(Location.X + inset, Location.Y + inset, fillWidth, height),
            };
        }
    }
}
