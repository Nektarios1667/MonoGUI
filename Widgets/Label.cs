using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xna = Microsoft.Xna.Framework;

namespace MonoGUI
{
    public class Label : Widget
    {
        public string Text { get; private set; }
        public Xna.Color Color { get; private set; }
        public SpriteFont? Font { get; private set; }
        // Centering
        public Label(GUI gui, Xna.Vector2 location, Color color, string text, SpriteFont? font = default) : base(gui, location)
        {
            Text = text;
            Font = font == default ? gui.Arial : font;
            Color = color;
        }
        public override void Update() { }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null) { return; }

            // Text
            Gui.Batch.DrawString(Font, Text, Location, Color);
        }
    }
}
