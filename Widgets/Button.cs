using MonoGame.Extended;

namespace MonoGUI
{
    public class Button : Widget
    {
        public Point Dimensions { get; private set; }
        public string Text { get; private set; }
        private string CutoffText { get; set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        }
        public Color Color { get; private set; }
        public Color Highlight { get; private set; }
        public SpriteFont? Font { get; private set; }
        public Color Foreground { get; private set; }
        public Delegate? Function { get; set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public object?[] Args { get; set; }
        public int State { get; private set; }
        public int Shift { get; private set; }
        public TextAlign Align { get; private set; }
        // Private
        private Vector2 offset;
        public Button(GUI gui, Point location, Point dimensions, Color foreground, Color color, Color highlight, Delegate? function, object?[] args, string text = "", TextAlign align = TextAlign.Middle, SpriteFont? font = default, int border = 3, Color borderColor = default, int shift = 0) : base(gui, location)
        {
            Dimensions = dimensions;
            Text = text;
            Font = font == default ? gui.Font : font;
            Align = align;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Function = function;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            Args = args;
            State = 0;
            Shift = shift;

            Vector2 inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            CutoffText = Font != null ? LimitString(text, Font, inside.X - 2) : Text;
            Vector2 textDim = Font != null ? Font.MeasureString(CutoffText) : new(0, 0);
            offset = Vector2.Floor((inside - textDim) / 2);
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            if (PointRectCollide(Location.ToVector2(), new Vector2(Dimensions.X - Border, Dimensions.Y - Border), Gui.MousePosition))
            {
                // Clicking
                if (Gui.LMouseClicked)
                {
                    State = 2;
                    if (Args.Length == 0)
                        Function?.DynamicInvoke();
                    else
                        Function?.DynamicInvoke(Args);
                }
                else { State = 1; }
            }
            else { State = 0; }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, State == 0 ? Color : Highlight);
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);

            // Text
            if (Font != null)
                if (Align == TextAlign.Left)
                    Gui.Batch.DrawString(Font, CutoffText, new(Location.X + Border + 2, Location.Y + Border + offset.Y + Shift), Foreground);
                else if (Align == TextAlign.Right)
                    Gui.Batch.DrawString(Font, CutoffText, new(Location.X + Dimensions.X - Border - Font.MeasureString(CutoffText).X - 2, Location.Y + Border + offset.Y + Shift), Foreground);
                else // Middle
                    Gui.Batch.DrawString(Font, CutoffText, new(Location.X + Border + offset.X + Shift, Location.Y + Border + offset.Y + Shift), Foreground);
            else if (Text != "")
                Console.WriteLine($"Skipping drawing text '{Text}' because of uninitialized font");
        }
        public override void Reload()
        {
            // Text dim
            Vector2 textDim = Font != null ? Font.MeasureString(Text) : new(0, 0);
            Vector2 inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            offset = Vector2.Floor((inside - textDim) / 2);
            // Cutoff text
            CutoffText = Font != null ? LimitString(Text, Font, inside.X) : Text;
        }
    }
}
