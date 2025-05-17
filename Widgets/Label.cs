using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xna = Microsoft.Xna.Framework;

namespace MonoGUI
{
    public class Label : Widget
    {
        public string Text { get; set; }
        public Xna.Color Color { get; set; }
        public SpriteFont? Font { get; set; }
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
