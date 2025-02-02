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
    }
}
