namespace MonoGUI;

/// <summary>Coordinates a set of radio buttons so that one option is selected at a time.</summary>
public sealed class RadioGroup
{
    private readonly List<RadioButton> _items = [];

    public IReadOnlyList<RadioButton> Items => _items;
    public RadioButton? Selected { get; private set; }
    public bool AllowDeselect { get; set; }
    public event Action<RadioButton?>? SelectionChanged;

    public void Add(RadioButton radioButton)
    {
        ArgumentNullException.ThrowIfNull(radioButton);
        if (_items.Contains(radioButton)) return;
        radioButton.Group?.Remove(radioButton);
        _items.Add(radioButton);
        radioButton.AttachToGroup(this);
        if (radioButton.IsSelected) Select(radioButton, notify: false);
    }

    public void Add(params RadioButton[] radioButtons)
    {
        ArgumentNullException.ThrowIfNull(radioButtons);
        foreach (RadioButton radioButton in radioButtons) Add(radioButton);
    }

    public bool Remove(RadioButton radioButton)
    {
        if (!_items.Remove(radioButton)) return false;
        radioButton.AttachToGroup(null);
        if (Selected != radioButton) return true;
        Selected = null;
        radioButton.SetSelected(false, notify: false);
        SelectionChanged?.Invoke(null);
        return true;
    }

    public void Select(RadioButton? radioButton, bool notify = true)
    {
        if (radioButton is not null && !_items.Contains(radioButton))
            throw new ArgumentException("The radio button does not belong to this group.", nameof(radioButton));
        if (Selected == radioButton) return;

        RadioButton? previous = Selected;
        Selected = radioButton;
        previous?.SetSelected(false, notify);
        radioButton?.SetSelected(true, notify);
        if (notify) SelectionChanged?.Invoke(Selected);
    }

    public void Clear(bool notify = true)
    {
        if (!AllowDeselect || Selected is null) return;
        Select(null, notify);
    }
}

/// <summary>A labelled, mutually-exclusive option. Add related instances to a <see cref="RadioGroup"/>.</summary>
public class RadioButton : Widget, ILinkable
{
    public string LinkableValue => IsSelected.ToString();
    public string Text { get; set; }
    public SpriteFont? Font { get; set; }
    public Color Foreground { get; set; }
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public Color SelectedColor { get; set; }
    public int Size { get; set; }
    public int Spacing { get; set; }
    public bool IsSelected { get; private set; }
    public RadioGroup? Group { get; private set; }
    public Rectangle Bounds => new(Location, new Point(Width, Math.Max(1, Size)));
    public event Action<bool>? ValueChanged;
    public event Action? Selected;

    public RadioButton(GUI gui, Point location, string text, Color foreground, Color color, Color highlight,
        RadioGroup? group = null, SpriteFont? font = null, int size = 20, int spacing = 6, Color? selectedColor = null) : base(gui, location)
    {
        Text = text ?? string.Empty;
        Font = font ?? gui.Font;
        Foreground = foreground;
        Color = color;
        Highlight = highlight;
        SelectedColor = selectedColor ?? foreground;
        Size = Math.Max(1, size);
        Spacing = Math.Max(0, spacing);
        group?.Add(this);
    }

    public override void Update()
    {
        if (!Visible || !Enabled) return;
        if (Gui.LMouseClicked && Bounds.Contains(Gui.MousePosition))
        {
            if (IsSelected && Group?.AllowDeselect == true) Group.Clear();
            else if (Group is not null) Group.Select(this);
            else SetSelected(true);
        }
    }

    public override void Draw()
    {
        if (!Visible) return;
        Color ringColor = Bounds.Contains(Gui.MousePosition) && Enabled ? Highlight : Color;
        Rectangle circleBounds = new(Location, new Point(Size));
        if (Gui.CircleOutline is not null)
        {
            Gui.Batch.Draw(Gui.CircleOutline, circleBounds, ringColor);
        }
        else
        {
            Gui.Batch.DrawRectangle(circleBounds, ringColor, Math.Max(1, Size / 8));
        }

        if (IsSelected)
        {
            int dotSize = Math.Max(3, Size / 3);
            Gui.Batch.FillRectangle(new Rectangle(Location.X + (Size - dotSize) / 2, Location.Y + (Size - dotSize) / 2, dotSize, dotSize), SelectedColor);
        }

        if (Font is not null && Text.Length > 0)
        {
            float textY = Location.Y + (Size - Font.LineSpacing) / 2f;
            Gui.Batch.DrawString(Font, Text, new Vector2(Location.X + Size + Spacing, textY), Foreground);
        }
    }

    public void SetSelected(bool selected, bool notify = true)
    {
        if (selected && Group is not null && Group.Selected != this)
        {
            Group.Select(this, notify);
            return;
        }
        if (IsSelected == selected) return;
        IsSelected = selected;
        if (notify) ValueChanged?.Invoke(IsSelected);
        if (selected && notify) Selected?.Invoke();
    }

    internal void AttachToGroup(RadioGroup? group) => Group = group;
    private int Width => Size + Spacing + (Font is null ? 0 : (int)MathF.Ceiling(Font.MeasureString(Text).X));
}
