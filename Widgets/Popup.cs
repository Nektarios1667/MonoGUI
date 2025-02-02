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
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace MonoGUI
{
    public class Popup : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        }
        public Xna.Color Color { get; private set; }
        public Delegate? Function { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public Color BarColor { get; private set; }
        public int BarSize { get; private set; }
        public Xna.Vector2 BarDimensions { get; private set; }
        public Xna.Vector2 LastBarPosition { get; private set; }
        public MouseState Previous { get; private set; }
        public Rectangle BarRect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)BarDimensions.X, (int)BarSize); }
        }
        private bool Dragging { get; set; }
        public List<Widget> Widgets { get; set; }
        public string Title { get; set; }
        public SpriteFont? TitleFont { get; set; }
        public Color TitleColor { get; set; }
        // Centering
        public Popup(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Color color, string title, SpriteFont? titleFont = default, Color titleColor = default, Color barColor = default, int barSize = 25, int border = 3, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Color = color;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : (Color)borderColor);
            BarColor = (barColor == default ? Color.Gray : (Color)barColor);
            BarSize = barSize;
            BarDimensions = new(dimensions.X, barSize);
            Visible = true;
            LastBarPosition = new(-1, -1);
            Previous = new();
            TitleColor = titleColor == default ? Color.Black : (Color)titleColor;
            Button closeButton = new(gui, new(location.X + dimensions.X - 50, location.Y), new(50, 25), Color.White, Color.Red, new(255, 75, 75), Close, args: [this]);
            TextBox titleBox = new(gui, new(location.X + 10, location.Y + 4), TitleColor, title, titleFont);
            Widgets = [closeButton, titleBox];
            TitleFont = titleFont == default ? gui.Arial : titleFont;
            Title = title;

        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            MouseState mouseState = Gui.MouseState;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Clicks
                if (PointRectCollide(Location, BarDimensions, mouseState.Position) || Dragging)
                {
                    // Dragging
                    if (Previous.LeftButton == ButtonState.Pressed && LastBarPosition.X != -1) {
                        Xna.Vector2 delta = mouseState.Position.ToVector2() - LastBarPosition;
                        foreach (Widget widget in Widgets) { widget.Location += delta; }
                        Location += delta;
                        Dragging = true;
                    }
                    LastBarPosition = mouseState.Position.ToVector2();
                }
            } else { Dragging = false; }

            // Widgets update
            foreach (Widget widget in Widgets) { widget.Update(); }

            // Previous
            Previous = mouseState;
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (TitleFont == null) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, Color);
            // Bar
            Gui.Batch.FillRectangle(BarRect, BarColor);
            // Widgets update
            foreach (Widget widget in Widgets) { widget.Draw(); }
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        }

        public void AddWidgets(params Widget[] widgets) { foreach (Widget widget in widgets) { Widgets.Add(widget); } }

        // static
        public static void Close(Popup popup) { popup.Visible = false; }
    }
}
