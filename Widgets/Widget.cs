using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Xna = Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace MonoGUI
{
    public abstract class Widget
    {
        public Xna.Vector2 Location { get; set; }
        public bool Visible { get; set; }
        public GUI Gui { get; set; }
        public Widget(GUI gui, Xna.Vector2 location) { Gui = gui; Location = location; Visible = true; }
        public abstract void Update();
        public abstract void Draw();
        public virtual void Reload() { }
        // Modify
        public void Modify(string property, object value, bool allowHidden = false)
        {
            // Get property
            PropertyInfo? propertyInfo = GetType().GetProperty(property);
            // If property does not exist or its hidden and allowHidden is disabled
            if (propertyInfo == null || (!char.IsUpper(property[0]) && !allowHidden)) { throw new ArgumentException($"{GetType()} widget does not have property {property}"); }

            // Check if new value and set
            if (!object.Equals(propertyInfo.GetValue(this), value)) { propertyInfo.SetValue(this, value); };

            // Relaod needed info
            Reload();
        }

        // Show and hide
        public virtual void Show() { Visible = true; }
        public virtual void Hide() { Visible = false; }
        public virtual void ToggleShow() { Visible = !Visible; }

        // Static methods

        // PointRectCollide
        public static bool PointRectCollide(Xna.Vector2 loc, Xna.Vector2 dim, Xna.Vector2 point)
        {
            return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
        }
        public static bool PointRectCollide(Xna.Point loc, Xna.Vector2 dim, Xna.Vector2 point)
        {
            return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
        }
        public static bool PointRectCollide(Xna.Point loc, Xna.Point dim, Xna.Vector2 point)
        {
            return (point.X >= loc.X && point.X <= loc.X + dim.X) && (point.Y >= loc.Y && point.Y <= loc.Y + dim.Y);
        }
        public static bool PointRectCollide(Xna.Vector2 loc, Xna.Vector2 dim, Xna.Point point)
        {
            return PointRectCollide(loc, dim, point.ToVector2());
        }
        public static bool PointRectCollide(Rectangle rect, Xna.Point point)
        {
            return PointRectCollide(rect.Location, rect.Size, point.ToVector2());
        }
        
        // PointCircleCollide
        public static bool PointCircleCollide(Xna.Vector2 loc, Xna.Vector2 center, int radius)
        {
            return Xna.Vector2.DistanceSquared(loc, center) <= radius * radius;
        }
        public static bool PointCircleCollide(Xna.Point loc, Xna.Vector2 center, int radius)
        {
            return PointCircleCollide(loc.ToVector2(), center, radius);
        }
        public static bool PointCircleCollide(Xna.Vector2 loc, Xna.Point center, int radius)
        {
            return PointCircleCollide(loc, center.ToVector2(), radius);
        }
        public static bool PointCircleCollide(Xna.Point loc, Xna.Point center, int radius)
        {
            return PointCircleCollide(loc.ToVector2(), center.ToVector2(), radius);
        }

        // Nofunc
        public static void NoFunc(params object[] _) {}

        // Softwraps
        public static string Softwrap(string text, SpriteFont font, Xna.Vector2 dimensions)
        {
            // setup
            string wrapped = "";
            int start = 0;
            int end = 1;

            while (end < text.Length)
            {
                // Wrap
                if (font.MeasureString(text[start..end]).X + 2 > dimensions.X) { wrapped += $"{text[start..end]}\n"; start = end; }
                end++;
            }
            return wrapped + text[start..end];
        }
        public static string SoftwrapWords(string text, SpriteFont font, Xna.Vector2 dimensions)
        {
            // setup
            string wrapped = "";
            int start = 0;
            int end = 1;

            while (end < text.Length)
            {
                // Wrap
                if (font.MeasureString(text[start..end]).X + 2 > dimensions.X)
                {
                    int cutoff = text[start..end].LastIndexOf(' ') + start;
                    if (cutoff <= start) { cutoff = end; }
                    wrapped += $"{text[start..cutoff]}\n";
                    start = cutoff + 1; // Add one to ignore the space itself
                    end = cutoff + 2;
                }
                end++;
            }
            wrapped += text[start..];
            return wrapped;
        }
        // Trims and ellipses
        public static string LimitString(string text, SpriteFont font, float width)
        {
            // If it fits
            if (font.MeasureString(text).X < width) { return text; }

            // Cutting off
            int end = text.Length - 1;
            while (text[..end].Length > 0 && font.MeasureString($"{text[..end]}...").X > width) { end--; }
            return $"{text[..end]}...";
        }
        public static string LimitLines(string text, SpriteFont font, float height)
        {
            // Height
            float lineHeight = font.MeasureString(text.Split('\n')[0]).Y;

            int maxLines = (int)Math.Max((height / lineHeight) - 1, 0);
            return string.Join('\n', text.Split('\n')[..maxLines]) + "\n...";
        }

        // Drawing
        public static void DrawX(SpriteBatch batch, Xna.Vector2 location, Xna.Vector2 dimensions, Color color, int thickness = 2)
        {
            //  "\" line
            batch.DrawLine(new(location.X - dimensions.X / 2, location.Y - dimensions.Y / 2), new(location.X + dimensions.X / 2, location.Y + dimensions.Y / 2), color, thickness);
            //  "/" line
            batch.DrawLine(new(location.X + dimensions.X / 2, location.Y - dimensions.Y / 2), new(location.X - dimensions.X / 2, location.Y + dimensions.Y / 2), color, thickness);
        }
    }
}
