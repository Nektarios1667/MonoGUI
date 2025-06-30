namespace MonoGUI
{
    public class InfoBox : Widget
    {
        public Point Dimensions { get; private set; }
        public Rectangle Rect { get; private set; }
        public Color Color { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public string Text { get; set; }
        public SpriteFont? Font { get; set; }
        public Color Foreground { get; set; }
        public Rectangle Activation { get; set; }
        public string Softwrapped { get; set; }
        public Point Inside { get; set; }
        public float Delay { get; set; }
        // Private
        private float time;
        public InfoBox(GUI gui, Point location, Point dimensions, Rectangle activation, Color color, Color foreground, string text, SpriteFont? font = default, float delay = 1, int border = 2, Color? borderColor = null) : base(gui, location)
        {
            Dimensions = dimensions;
            Activation = activation;
            Color = color;
            Border = border;
            BorderColor = (borderColor == null ? Color.Black : (Color)borderColor);
            Visible = true;
            Text = text;
            Font = font == default ? gui.Font : font;
            Foreground = foreground;
            Delay = delay;

            // Other variables
            Inside = new(Dimensions.X - Border * 2 - 2, Dimensions.Y - Border * 2 - 2);
            Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
            Softwrapped = Font != null ? LimitLines(SoftwrapWords(Text, Font, Inside), Font, Inside.Y) : Text;
        }
        public override void Update()
        {
            // Hovering
            if (PointRectCollide(Activation, Gui.MousePosition))
            {
                if (time >= Delay) { Visible = true; }
                else { time += Gui.Delta; }
            }
            else { Visible = false; time = 0; }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, Color);
            // Text
            Gui.Batch.DrawString(Font, Softwrapped, new(Location.X + Border + 2, Location.Y + Border + 2), Foreground);
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        }
        public override void Reload()
        {
            Softwrapped = Font != null ? LimitLines(SoftwrapWords(Text, Font, Inside), Font, Inside.Y) : Text;
            Inside = new(Dimensions.X - Border * 2, Dimensions.Y - Border * 2);
            Rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
        }
    }
}
