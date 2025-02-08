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
using System.Collections;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace MonoGUI
{
    public class Input : Widget
    {
        public Xna.Vector2 Dimensions { get; private set; }
        public string Text { get; set; }
        public Rectangle Rect
        {
            get { return new((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y); }
        }
        public Xna.Color Color { get; private set; }
        public Xna.Color Highlight { get; private set; }
        public SpriteFont? Font { get; private set; }
        public Color Foreground { get; private set; }
        public Delegate? Function { get; private set; }
        public int Border { get; private set; }
        public Color BorderColor { get; private set; }
        public object?[]? Args { get; private set; }
        public bool Selected { get; private set; }
        public int Cursor { get; private set; }
        // Private
        private Keys[] previousKeys;
        private MouseState previousState;
        private float blink = 0;
        private int cursorX = 0;
        private int textsize = 0;
        private Xna.Vector2 charsize;
        // Key mapping
        private readonly Dictionary<string, char> specialKeys = new()
        {
            ["OemPeriod"] = '.',
            ["OemComma"] = ',',
            ["OemQuestion"] = '/',
            ["OemSemicolon"] = ';',
            ["OemQuotes"] = '\'',
            ["OemPlus"] = '=',
            ["OemMinus"] = '-',
            ["OemPipe"] = '\\',
            ["OemOpenBrackets"] = '[',
            ["OemCloseBrackets"] = ']',
            ["OemTilde"] = '`',
            ["Space"] = ' ',
            ["D1"] = '1',
            ["D2"] = '2',
            ["D3"] = '3',
            ["D4"] = '4',
            ["D5"] = '5',
            ["D6"] = '6',
            ["D7"] = '7',
            ["D8"] = '8',
            ["D9"] = '9',
            ["D0"] = '0',
        };
        private readonly Dictionary<string, char> upperSymbols = new()
        {
            ["D1"] = '!',
            ["D2"] = '@',
            ["D3"] = '#',
            ["D4"] = '$',
            ["D5"] = '%',
            ["D6"] = '^',
            ["D7"] = '&',
            ["D8"] = '*',
            ["D9"] = '(',
            ["D0"] = ')',
            ["OemPeriod"] = '>',
            ["OemComma"] = '<',
            ["OemQuestion"] = '?',
            ["OemSemicolon"] = ':',
            ["OemQuotes"] = '"',
            ["OemPlus"] = '+',
            ["OemMinus"] = '_',
            ["OemPipe"] = '|',
            ["OemOpenBrackets"] = '{',
            ["OemCloseBrackets"] = '}',
            ["OemTilde"] = '~',
        };
        private string[] controlKeys = [ "Back", "Left", "Right" ];
        public Input(GUI gui, Xna.Vector2 location, Xna.Vector2 dimensions, Color foreground, Xna.Color color, Xna.Color highlight, SpriteFont? font = default, int border = 3, Color borderColor = default) : base(gui, location)
        {
            Dimensions = dimensions;
            Text = "";
            charsize = font != null ? font.MeasureString("_") : new(0, 0);
            Font = font == default ? gui.Arial : font;
            Foreground = foreground;
            Color = color;
            Highlight = highlight;
            Border = border;
            BorderColor = borderColor == default ? Color.Black : borderColor;
            Selected = false;
            Cursor = 0;
            previousKeys = [];
        }
        public override void Update()
        {
            // Hidden
            if (!Visible) { return; }
            if (Font == null) { return; }

            // Blink
            if (Selected) { blink = (blink + Gui.Delta) % 1f; }
            else blink = .499f;

            // Clicking
            MouseState mouseState = Gui.MouseState;
            if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton != ButtonState.Pressed)
            {                
                // Checks
                if (PointRectCollide(Location, Dimensions, mouseState.Position)) { Selected = true; }
                else { Selected = false; }
            }
            previousState = mouseState;

            // Typing
            Keys[] pressed = Gui.KeyState.GetPressedKeys();
            if (!Selected) { return; }
            bool shifted = pressed.Contains(Keys.LeftShift) || pressed.Contains(Keys.RightShift);
            char specialKeyname, specialUpperKeyname;
            foreach (Keys key in pressed)
            {
                string keyname = key.ToString();
                if (!previousKeys.Contains(key) && (textsize + Font.MeasureString(ParseRegularChar(keyname, shifted)).X < Dimensions.X - 2 * Border || controlKeys.Contains(keyname)))
                {
                    // Uppercase letter
                    if (keyname.Length == 1 && shifted) { Text = Text.Insert(Cursor, keyname); }
                    // Lowercase letter
                    else if (keyname.Length == 1) { Text = Text.Insert(Cursor, keyname.ToLower()); }
                    // Lowercase symbol
                    else if (!shifted && specialKeys.TryGetValue(keyname, out specialKeyname)) { Text = Text.Insert(Cursor, specialKeyname.ToString()); ; }
                    // Uppercase symbol
                    else if (shifted && specialKeys.ContainsKey(keyname) && upperSymbols.TryGetValue(keyname, out specialUpperKeyname)) { Text = Text.Insert(Cursor, specialUpperKeyname.ToString()); }
                    // Backspace
                    else if (keyname == "Back" && Text.Length > 0) { Text = Text.Remove(Cursor - 1, 1); }
                    // Move cursor right
                    else if (keyname == "Right" && Cursor < Text.Length) { Cursor++; }
                    // Move cursor left
                    else if (keyname == "Left" && Cursor > 0) { Cursor--; }
                    // Continue
                    else { continue; }
                    // Every taken key other than backspace, left arrow, and right arrow adds a char
                    if (keyname == "Back") { Cursor--; }
                    else if (!controlKeys.Contains(keyname)) { Cursor++; }
                    Reload();
                }
            }
            previousKeys = pressed;
        }
        public override void Draw()
        {
            // Not drawing
            if (!Visible) { return; }

            // Background
            Gui.Batch.FillRectangle(Rect, Selected ? Highlight : Color);
            // Outline
            Gui.Batch.DrawRectangle(Rect, BorderColor);

            // Text
            if (Font != null)
            {
                Gui.Batch.DrawString(Font, Text, new(Location.X + Border, Location.Y + Border), Foreground);
            } else if (Text != "")
            {
                Console.WriteLine($"Skipping drawing text '{Text}' because of uninitialized font");
            }

            // Cursor
            if (blink >= .5) { Gui.Batch.DrawLine(Location.X + Border + cursorX, Location.Y + Border + 1, Location.X + Border + cursorX, Location.Y + charsize.Y + 2, Color.Black, 2); }
        }
        public override void Reload()
        {
            charsize = Font != null ? Font.MeasureString("_") : new(0, 0);
            // Recalculate text size
            textsize = Text.Length > 0 ? (int)Font.MeasureString(Text).X : 0;
            cursorX = Font != null ? (int)Font.MeasureString(Text[..Cursor]).X : 0;
            blink = .51f;
        }
        public string ParseRegularChar(string keyname, bool shifted)
        {
            char specialKeyname, specialUpperKeyname;
            // Uppercase letter
            if (keyname.Length == 1 && shifted) { return keyname; }
            // Lowercase letter
            else if (keyname.Length == 1) { return keyname.ToLower(); }
            // Lowercase symbol
            else if (!shifted && specialKeys.TryGetValue(keyname, out specialKeyname)) { return specialKeyname.ToString(); }
            // Uppercase symbol
            else if (shifted && specialKeys.ContainsKey(keyname) && upperSymbols.TryGetValue(keyname, out specialUpperKeyname)) { return specialUpperKeyname.ToString(); }
            return "";
        }

        // Static
        public int TextMeasure(SpriteFont font, char character) { return (int)font.MeasureString(character.ToString()).X; }
        public int TextMeasure(SpriteFont font, string character) { return (int)font.MeasureString(character).X; }
    }
}
