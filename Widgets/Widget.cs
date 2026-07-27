using System.Reflection;

namespace MonoGUI;

public abstract class Widget
{
    public Point Location { get; set; }
    public bool Visible { get; set; } = true;
    public bool Enabled { get; set; } = true;
    public GUI Gui { get; set; }
    public Widget(GUI gui, Point location) { Gui = gui; Location = location; }
    public abstract void Update();
    public abstract void Draw();
    public virtual void Reload() { }
    // Modify
    public void Modify(string property, object value, bool allowHidden = false)
    {
        // Get property
        ArgumentException.ThrowIfNullOrWhiteSpace(property);
        PropertyInfo? propertyInfo = GetType().GetProperty(property);
        // If property does not exist or its hidden and allowHidden is disabled
        if (propertyInfo == null || !propertyInfo.CanWrite || (!char.IsUpper(property[0]) && !allowHidden)) { throw new ArgumentException($"{GetType()} widget does not have a writable property named {property}.", nameof(property)); }

        // Check if new value and set
        if (!Equals(propertyInfo.GetValue(this), value)) { propertyInfo.SetValue(this, value); }

        // Relaod needed info
        Reload();
    }

    // Show and hide
    public virtual void Show() { Visible = true; }
    public virtual void Hide() { Visible = false; }
    public virtual void ToggleShow() { Visible = !Visible; }

    // Static methods

    // PointRectCollide
    public static bool PointRectCollide(Vector2 loc, Vector2 dim, Vector2 point)
    {
        return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
    }
    public static bool PointRectCollide(Point loc, Vector2 dim, Vector2 point)
    {
        return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
    }
    public static bool PointRectCollide(Point loc, Point dim, Vector2 point)
    {
        return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
    }
    public static bool PointRectCollide(Point loc, Point dim, Point point)
    {
        return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
    }
    public static bool PointRectCollide(Vector2 loc, Vector2 dim, Point point)
    {
        return PointRectCollide(loc, dim, point.ToVector2());
    }
    public static bool PointRectCollide(Rectangle rect, Point point)
    {
        return PointRectCollide(rect.Location, rect.Size, point.ToVector2());
    }

    // PointCircleCollide
    public static bool PointCircleCollide(Vector2 loc, Vector2 center, int radius)
    {
        return Vector2.DistanceSquared(loc, center) <= radius * radius;
    }
    public static bool PointCircleCollide(Point loc, Vector2 center, int radius)
    {
        return PointCircleCollide(loc.ToVector2(), center, radius);
    }
    public static bool PointCircleCollide(Vector2 loc, Point center, int radius)
    {
        return PointCircleCollide(loc, center.ToVector2(), radius);
    }
    public static bool PointCircleCollide(Point loc, Point center, int radius)
    {
        return PointCircleCollide(loc.ToVector2(), center.ToVector2(), radius);
    }

    // Nofunc
    public static void NoFunc() { }

    // Softwraps
    public static string Softwrap(string text, SpriteFont font, Point dimensions)
    {
        // setup
        if (string.IsNullOrEmpty(text) || dimensions.X <= 0) return text;
        List<string> lines = [];
        string line = string.Empty;
        foreach (char character in text)
        {
            if (character == '\n') { lines.Add(line); line = string.Empty; continue; }
            string candidate = line + character;
            if (line.Length > 0 && font.MeasureString(candidate).X + 2 > dimensions.X) { lines.Add(line); line = character.ToString(); }
            else line = candidate;
        }
        lines.Add(line);
        return string.Join('\n', lines);
    }
    public static string SoftwrapWords(string text, SpriteFont font, Point dimensions)
    {
        if (string.IsNullOrEmpty(text) || dimensions.X <= 0) return text;
        List<string> lines = [];
        foreach (string paragraph in text.Split('\n'))
        {
            string line = string.Empty;
            foreach (string word in paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                string candidate = line.Length == 0 ? word : $"{line} {word}";
                if (line.Length > 0 && font.MeasureString(candidate).X + 2 > dimensions.X) { lines.Add(line); line = word; }
                else line = candidate;
            }
            lines.Add(line);
        }
        return string.Join('\n', lines);
    }
    // Trims and ellipses
    public static string LimitString(string text, SpriteFont font, float width)
    {
        // If it fits
        if (width <= 0) return string.Empty;
        if (font.MeasureString(text).X <= width) { return text; }

        // Cutting off
        const string ellipsis = "...";
        if (font.MeasureString(ellipsis).X > width) return string.Empty;
        int end = text.Length;
        while (end > 0 && font.MeasureString($"{text[..end]}{ellipsis}").X > width) { end--; }
        return $"{text[..end]}{ellipsis}";
    }
    public static string LimitLines(string text, SpriteFont font, float height)
    {
        // Height
        string[] lines = text.Split('\n');
        if (lines.Length == 0 || height <= 0) return string.Empty;
        float lineHeight = font.LineSpacing;
        int maxLines = Math.Max((int)(height / lineHeight), 0);
        if (maxLines >= lines.Length) return text;
        if (maxLines == 0) return string.Empty;
        return maxLines == 1 ? "..." : string.Join('\n', lines[..(maxLines - 1)]) + "\n...";
    }

    // Drawing
    public static void DrawX(SpriteBatch batch, Point location, Point dimensions, Color color, int thickness = 2)
    {
        //  "\" line
        batch.DrawLine(new(location.X - dimensions.X / 2, location.Y - dimensions.Y / 2), new(location.X + dimensions.X / 2, location.Y + dimensions.Y / 2), color, thickness);
        //  "/" line
        batch.DrawLine(new(location.X + dimensions.X / 2, location.Y - dimensions.Y / 2), new(location.X - dimensions.X / 2, location.Y + dimensions.Y / 2), color, thickness);
    }
}
