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
using static System.Runtime.InteropServices.JavaScript.JSType;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace MonoGUI
{
    public class Dropdown : Widget
    {
        public event Action<string> ItemSelected;
        public Xna.Vector2 Dimensions { get; private set; }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public SpriteFont? Font { get; private set; }
        public Color Foreground { get; private set; }
        public int Seperation { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public List<Button> Items { get; private set; }
        public string Selected { get; private set; }
        private int itemHeight { get; set; }
        public bool Open { get; set; }
        public Button SelectButton { get; private set; }
        // Centering
        private Xna.Vector2 offset { get; set; }
        private bool Last { get; set; }
        public Dropdown(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Color foreground, Xna.Color color, Xna.Color highlight, SpriteFont? font = default, int seperation = 2, int border = 2, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Items = [];
            Selected = "";
            Font = font == default ? gui.Arial : font;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Seperation = seperation;
            Border = border;
            BorderColor = (borderColor == default ? Color.Black : borderColor);
            SelectButton = new(Gui, location, dimensions, foreground, color, highlight, ToggleOpen, Selected, font: Font, border: Border, borderColor: BorderColor);
            itemHeight = Font != null ? (int)Font.MeasureString("|").Y + 4 + Seperation * 2 : 0;
            Open = false;
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }
            
            // Update selection button
            SelectButton.Update();

            // Update items
            if (Open)
            {
                foreach (Button item in Items) { item.Update(); }
            }
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }
            if (Font == null || Gui.ArrowDown == null) { return; }

            Rectangle rect = new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);

            // Draw selection button
            SelectButton.Draw();

            // Draw items
            if (Open)
            {
                foreach (Button item in Items) { item.Draw(); }
            }

            // Draw arrows
            int arrowSize = (int)Math.Min(Dimensions.X, Dimensions.Y) / 2;
            Xna.Vector2 arrowLocation = new(Location.X + Dimensions.X - Border - arrowSize, Location.Y + Dimensions.Y / 2);
            float scale = Math.Min(Dimensions.X, Dimensions.Y) / 40f;
            Gui.Batch.Draw(Gui.ArrowDown, arrowLocation, null, BorderColor, Open ? 0 : -(float)(Math.PI / 2), new(8, 5), scale, SpriteEffects.None, 0f);
        }
        public void AddItems(params string[] texts)
        {
            foreach (string text in texts)
            {
                Xna.Vector2 loc = new(Location.X + Border, Location.Y + Dimensions.Y - Seperation + itemHeight * Items.Count);
                Xna.Vector2 dim = new(Dimensions.X - Border - Seperation, itemHeight);
                Items.Add(new(Gui, loc, dim, Foreground, Color, Highlight, SelectItem, text, Font, args: [text], border: Seperation, borderColor: BorderColor));
            }
        }
        public void ToggleOpen() { Open = !Open; }
        public void SelectItem(string item) { Selected = item; SelectButton.SetText(item);  OnItemSelected(item); Open = false; }
        // When changed value
        public virtual void OnItemSelected(string item)
        {
            ItemSelected?.Invoke(item); // Invoke the event if any listeners are attached
        }
    }
}
