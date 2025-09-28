namespace MonoGUI
{
    public class Checkbox : Widget
    {
        public int Size { get; private set; }
        public Rectangle Rect
        {
            get { return new(Location.X, Location.Y, Size, Size); }
        }
        public event Action<bool> ValueChanged;
        public Color Color { get; private set; }
        public Color Highlight { get; private set; }
        public Color Foreground { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public bool Checked { get; set; }
        public int State { get; private set; }
        public int CheckThickness { get; private set; }
        private Point CheckLocation { get; set; }
        private Point CheckDimensions { get; set; }
        public Checkbox(GUI gui, Point location, int size, Color foreground, Color color, Color highlight, int border = 3, Color borderColor = default, int checkThickness = 4) : base(gui, location)
        {
            Size = size;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            Checked = false;
            CheckThickness = checkThickness;
            CheckLocation = new(Location.X + Size / 2, Location.Y + Size / 2);
            int checkSize = Size - CheckThickness - Border * 2;
            CheckDimensions = new(checkSize, checkSize);
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }

            // Hovering
            if (PointRectCollide(Location, new Point(Size), Gui.MousePosition.ToVector2()))
            {
                // Clicking
                if (Gui.LMouseClicked)
                {
                    Checked = !Checked;
                    ValueChanged?.Invoke(Checked);
                    State = 2;
                }
                else { State = 1; }
            }
            else { State = 0; }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, State == 0 ? Color : Highlight);
            // Draw X
            if (Checked)
            {
                DrawX(Gui.Batch, CheckLocation, CheckDimensions, Foreground, thickness: CheckThickness);
            }
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor, Border);
        }
        public override void Reload()
        {
            // Check
            CheckLocation = new(Location.X + Size / 2, Location.Y + Size / 2);
            int checkSize = Size - CheckThickness - Border * 2;
            CheckDimensions = new(checkSize, checkSize);
        }
    }
}
