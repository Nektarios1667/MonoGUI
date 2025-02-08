using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Xna = Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Reflection;
using System.Collections.Generic;

namespace MonoGUI
{
    public class Popup : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public Rectangle Rect { get; private set; }
        public Xna.Color Color { get; private set; }
        public Delegate? Function { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public Color BarColor { get; private set; }
        public int BarSize { get; private set; }
        public Xna.Vector2 BarDimensions { get; private set; }
        public Xna.Vector2 LastBarPosition { get; private set; }
        public MouseState Previous { get; private set; }
        public Rectangle BarRect { get; private set; }
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
            Previous = default;
            TitleColor = titleColor == default ? Color.Black : (Color)titleColor;
            TitleFont = titleFont == default ? gui.Arial : titleFont;
            Title = title;

            // Builtin
            Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
            BarRect = new((int)Location.X, (int)Location.Y, (int)BarDimensions.X, (int)BarSize);
            Label titleBox = new(Gui, new(Location.X + 10, Location.Y + 4), TitleColor, LimitString(Title, TitleFont, Dimensions.X - 50 - Border * 2 - 10), TitleFont);
            Button closeButton = new(Gui, new(Location.X + Dimensions.X - 50, Location.Y), new(50, 25), Color.White, Color.Red, new(255, 75, 75), Close, args: [this]);
            Widgets = [closeButton, titleBox];

        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Clicking
            MouseState mouseState = Gui.MouseState;
            if (mouseState.LeftButton == ButtonState.Pressed && (Previous.LeftButton != ButtonState.Pressed || Dragging))
            {
                // Dragging
                if (PointRectCollide(BarRect, mouseState.Position) || Dragging)
                {
                    if (Previous != default) // Check if the first time clicking
                    {
                        // Move items
                        Xna.Vector2 delta = mouseState.Position.ToVector2() - Previous.Position.ToVector2();
                        foreach ( Widget widget in Widgets) { widget.Location += delta; }

                        // Update self
                        Location += delta;
                        Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
                        BarRect = new((int)Location.X, (int)Location.Y, (int)BarDimensions.X, (int)BarSize);

                        // Drag
                        Dragging = true;
                    }
                } else if (!PointRectCollide(Rect, mouseState.Position) && Previous.LeftButton != ButtonState.Pressed) { Visible = false; }
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
        public override void Reload()
        {
            Label titleBox = new(Gui, new(Location.X + 10, Location.Y + 4), TitleColor, LimitString(Title, TitleFont, Dimensions.X - 50 - Border * 2 - 10), TitleFont);
            Button closeButton = new(Gui, new(Location.X + Dimensions.X - 50, Location.Y), new(50, 25), Color.White, Color.Red, new(255, 75, 75), Close, args: [this]);
            Widgets = [closeButton, titleBox];
        }
        public void AddWidgets(params Widget[] widgets) { foreach (Widget widget in widgets) { Widgets.Add(widget); } }

        // static
        public static void Close(Popup popup) { popup.Visible = false; }
    }
}
