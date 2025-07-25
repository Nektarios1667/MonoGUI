﻿namespace MonoGUI
{
    public class Popup : Widget
    {
        public event Action? OnClose;
        public Point Dimensions { get; private set; }
        public Rectangle Rect { get; private set; }
        public Color Color { get; private set; }
        public Delegate? Function { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public Color BarColor { get; private set; }
        public int BarSize { get; private set; }
        public Vector2 BarDimensions { get; private set; }
        public MouseState PreviousState { get; private set; }
        public Rectangle BarRect { get; private set; }
        public List<Widget> Widgets { get; set; }
        public string Title { get; set; }
        public SpriteFont TitleFont { get; set; }
        public Color TitleColor { get; set; }
        // Private
        private bool dragging = false;
        private Button closeButton;
        public Popup(GUI gui, Point location, Point dimensions, Color color, string title, SpriteFont? titleFont = default, Color titleColor = default, Color barColor = default, int barSize = 25, int border = 3, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Color = color;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : (Color)borderColor);
            BarColor = (barColor == default ? Color.Gray : (Color)barColor);
            BarSize = barSize;
            BarDimensions = new(dimensions.X, barSize);
            Visible = true;
            PreviousState = new();
            TitleColor = titleColor == default ? Color.Black : (Color)titleColor;
            TitleFont = titleFont == default ? gui.Font! : titleFont;
            Title = title;

            // Builtin
            Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
            BarRect = new((int)Location.X, (int)Location.Y, (int)BarDimensions.X, (int)BarSize);
            closeButton = new(Gui, new(Location.X + Dimensions.X - 50, Location.Y), new(50, 25), Color.White, Color.Red, new(255, 75, 75), Close, [this]);
            Widgets = [];

        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Clicking
            MouseState mouseState = Gui.MouseState;
            if (mouseState.LeftButton == ButtonState.Pressed && (PreviousState.LeftButton != ButtonState.Pressed || dragging))
            {
                // Dragging
                if (PointRectCollide(BarRect, mouseState.Position) || dragging)
                {
                    if (PreviousState != default) // Check if the first time clicking
                    {
                        // Move items
                        Point delta = (mouseState.Position.ToVector2() - PreviousState.Position.ToVector2()).ToPoint();
                        foreach (Widget widget in Widgets) { widget.Location += delta; }
                        closeButton.Location += delta;

                        // Update self
                        Location += delta;
                        Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
                        BarRect = new((int)Location.X, (int)Location.Y, (int)BarDimensions.X, (int)BarSize);

                        // Drag
                        dragging = true;
                    }
                }
                else if (!PointRectCollide(Rect, mouseState.Position) && PreviousState.LeftButton != ButtonState.Pressed) { Close(this); }
            }
            else { dragging = false; }

            // Widgets update
            foreach (Widget widget in Widgets) { widget.Update(); }
            closeButton.Update();

            // previousState
            PreviousState = mouseState;
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
            // Builtins
            closeButton.Draw();
            string text = LimitString(Title, TitleFont, Dimensions.X - Border * 2 - closeButton.Dimensions.X - closeButton.Border * 2);
            Gui.Batch.DrawString(TitleFont, text, new Vector2(Location.X + 10, Location.Y + 4), TitleColor);
        }
        public override void Reload()
        {
            Label titleBox = new(Gui, new(Location.X + 10, Location.Y + 4), TitleColor, LimitString(Title, TitleFont, Dimensions.X - 50 - Border * 2 - 10), TitleFont);
            Button closeButton = new(Gui, new(Location.X + Dimensions.X - 50, Location.Y), new(50, 25), Color.White, Color.Red, new(255, 75, 75), Close, [this]);
            Widgets = [closeButton, titleBox];
        }
        // static
        public static void Close(Popup popup) { popup.Visible = false; popup.OnClose?.Invoke(); }
    }
}
