namespace MonoGUI;

/// <summary>A single-line text editor with keyboard repeat, a caret, placeholder text, and change notifications.</summary>
public class Input : Widget, ILinkable
{
    private const float InitialRepeatDelay = 0.45f;
    private const float RepeatInterval = 0.05f;
    private readonly Dictionary<Keys, float> _heldKeys = [];
    private float _blink;
    private int _cursorX;
    private Vector2 _characterSize;

    public string LinkableValue => Text;
    public Point Dimensions { get; set; }
    public string Text { get; private set; } = string.Empty;
    public Rectangle Rect => new(Location, Dimensions);
    public Color Color { get; set; }
    public Color Highlight { get; set; }
    public SpriteFont? Font { get; set; }
    public Color Foreground { get; set; }
    public Color PlaceholderColor { get; set; } = Color.Gray;
    public string Placeholder { get; set; } = string.Empty;
    public int Border { get; set; }
    public Color BorderColor { get; set; }
    public bool Selected { get; private set; }
    public int Cursor { get; private set; }
    public int MaxLength { get; set; }
    public event Action<string>? TextChanged;
    public event Action<string>? Submitted;

    public Input(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight,
        SpriteFont? font = default, int border = 3, Color borderColor = default) : base(gui, location)
    {
        if (dimensions.X <= 0 || dimensions.Y <= 0) throw new ArgumentOutOfRangeException(nameof(dimensions));
        Dimensions = dimensions;
        Font = font ?? gui.Font;
        Foreground = foreground;
        Color = color;
        Highlight = highlight;
        Border = Math.Max(0, border);
        BorderColor = borderColor == default ? Microsoft.Xna.Framework.Color.Black : borderColor;
        Reload();
    }

    public override void Update()
    {
        if (!Visible || !Enabled || Font is null) return;

        if (Gui.LMouseClicked)
        {
            Selected = Rect.Contains(Gui.MousePosition);
            if (Selected) Cursor = GetCursorAt(Gui.MousePosition.X - Location.X - Border);
            _blink = 0f;
        }

        if (!Selected) return;
        _blink = (_blink + Gui.Delta) % 1f;
        bool shift = Gui.AnyKeyDown(Keys.LeftShift, Keys.RightShift);
        bool capsLock = Gui.KeyDown(Keys.CapsLock);

        foreach (Keys key in Gui.KeysPressed)
        {
            if (!ShouldHandle(key)) continue;
            HandleKey(key, shift, capsLock);
        }
        foreach (Keys releasedKey in _heldKeys.Keys.Except(Gui.KeysPressed).ToArray()) _heldKeys.Remove(releasedKey);
    }

    public override void Draw()
    {
        if (!Visible) return;
        Gui.Batch.FillRectangle(Rect, Selected ? Highlight : Color);
        Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        if (Font is null) return;

        if (Text.Length > 0) Gui.Batch.DrawString(Font, Text, new Vector2(Location.X + Border, Location.Y + Border), Foreground);
        else if (!Selected && Placeholder.Length > 0) Gui.Batch.DrawString(Font, LimitString(Placeholder, Font, InnerWidth), new Vector2(Location.X + Border, Location.Y + Border), PlaceholderColor);

        if (Selected && _blink >= 0.5f)
        {
            float height = Math.Min(_characterSize.Y, Dimensions.Y - Border * 2);
            Gui.Batch.DrawLine(new Vector2(Location.X + Border + _cursorX, Location.Y + Border + 1), new Vector2(Location.X + Border + _cursorX, Location.Y + Border + height), Foreground, 1);
        }
    }

    public override void Reload()
    {
        Cursor = Math.Clamp(Cursor, 0, Text.Length);
        _characterSize = Font?.MeasureString("_") ?? Vector2.Zero;
        _cursorX = Font is null ? 0 : (int)Font.MeasureString(Text[..Cursor]).X;
        _blink = 0f;
    }

    public void SetText(string? text, bool notify = true)
    {
        string newText = text ?? string.Empty;
        if (MaxLength > 0 && newText.Length > MaxLength) newText = newText[..MaxLength];
        if (Text == newText) { Reload(); return; }
        Text = newText;
        Reload();
        if (notify) TextChanged?.Invoke(Text);
    }

    public void Clear(bool notify = true) => SetText(string.Empty, notify);

    [Obsolete("Use SetText. This compatibility method is retained for existing callers.")]
    public string ParseRegularChar(string keyname, bool shifted) => TryGetCharacter(Enum.TryParse(keyname, out Keys key) ? key : Keys.None, shifted, false, out char character) ? character.ToString() : string.Empty;

    public static int TextMeasure(SpriteFont font, char character) => (int)font.MeasureString(character.ToString()).X;
    public static int TextMeasure(SpriteFont font, string text) => (int)font.MeasureString(text).X;

    private float InnerWidth => Math.Max(0, Dimensions.X - Border * 2);

    private bool ShouldHandle(Keys key)
    {
        if (!Gui.LastKeyState.IsKeyDown(key)) { _heldKeys[key] = 0; return true; }
        if (!_heldKeys.TryGetValue(key, out float held)) held = 0;
        held += Gui.Delta;
        _heldKeys[key] = held;
        if (held < InitialRepeatDelay) return false;
        float previous = held - Gui.Delta - InitialRepeatDelay;
        return (int)(previous / RepeatInterval) != (int)((held - InitialRepeatDelay) / RepeatInterval);
    }

    private void HandleKey(Keys key, bool shift, bool capsLock)
    {
        switch (key)
        {
            case Keys.Back:
                if (Cursor > 0) Replace(Text.Remove(Cursor - 1, 1), Cursor - 1);
                return;
            case Keys.Delete:
                if (Cursor < Text.Length) Replace(Text.Remove(Cursor, 1), Cursor);
                return;
            case Keys.Left:
                Cursor = Math.Max(0, Cursor - 1); Reload(); return;
            case Keys.Right:
                Cursor = Math.Min(Text.Length, Cursor + 1); Reload(); return;
            case Keys.Home:
                Cursor = 0; Reload(); return;
            case Keys.End:
                Cursor = Text.Length; Reload(); return;
            case Keys.Enter:
                Submitted?.Invoke(Text); return;
        }

        if (!TryGetCharacter(key, shift, capsLock, out char character) || (MaxLength > 0 && Text.Length >= MaxLength)) return;
        string proposed = Text.Insert(Cursor, character.ToString());
        if (Font is not null && Font.MeasureString(proposed).X > InnerWidth) return;
        Replace(proposed, Cursor + 1);
    }

    private void Replace(string newText, int cursor)
    {
        if (Text == newText) return;
        Text = newText;
        Cursor = cursor;
        Reload();
        TextChanged?.Invoke(Text);
    }

    private int GetCursorAt(float x)
    {
        if (Font is null || x <= 0) return 0;
        for (int index = 1; index <= Text.Length; index++)
            if (Font.MeasureString(Text[..index]).X >= x) return index;
        return Text.Length;
    }

    private static bool TryGetCharacter(Keys key, bool shift, bool capsLock, out char character)
    {
        character = default;
        if (key is >= Keys.A and <= Keys.Z)
        {
            char letter = (char)('a' + (key - Keys.A));
            character = shift ^ capsLock ? char.ToUpperInvariant(letter) : letter;
            return true;
        }
        string normal = "1234567890";
        string shifted = "!@#$%^&*()";
        if (key is >= Keys.D0 and <= Keys.D9)
        {
            int index = key - Keys.D0;
            character = shift ? shifted[index] : normal[index];
            return true;
        }
        character = key switch
        {
            Keys.Space => ' ', Keys.OemPeriod => shift ? '>' : '.', Keys.OemComma => shift ? '<' : ',',
            Keys.OemQuestion => shift ? '?' : '/', Keys.OemSemicolon => shift ? ':' : ';', Keys.OemQuotes => shift ? '"' : '\'',
            Keys.OemPlus => shift ? '+' : '=', Keys.OemMinus => shift ? '_' : '-', Keys.OemPipe => shift ? '|' : '\\',
            Keys.OemOpenBrackets => shift ? '{' : '[', Keys.OemCloseBrackets => shift ? '}' : ']', Keys.OemTilde => shift ? '~' : '`',
            _ => default
        };
        return character != default;
    }
}
