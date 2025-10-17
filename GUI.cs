global using System;
global using System.Collections.Generic;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Content;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
global using MonoGame.Extended;

using System.IO;
using System.Linq;


namespace MonoGUI;

public enum TextAlign
{
    Left,
    Middle,
    Right,
}
public class GUI
{
    // Colors
    public static readonly Color NearBlack = new(55, 55, 55);
    // Input generators
    public Point MousePosition => MouseState.Position;
    public Keys[] KeysPressed => KeyState.GetPressedKeys();
    public bool KeyPressed(Keys key) => KeyState.IsKeyDown(key) && LastKeyState.IsKeyUp(key);
    public bool KeyDown(Keys key) => KeyState.IsKeyDown(key);
    public bool AnyKeyDown(params Keys[] keys)
    {
        foreach (Keys key in keys)
            if (KeyState.IsKeyDown(key)) return true;
        return false;
    }
    public bool AllKeysDown(params Keys[] keys)
    {
        foreach (Keys key in keys)
            if (!KeyState.IsKeyDown(key)) return false;
        return true;
    }
    public bool Hotkey(Keys modifier, Keys key) => KeyState.IsKeyDown(modifier) && KeyPressed(key);
    public bool Hotkey(Keys modifier1, Keys modifier2, Keys key) => KeyState.IsKeyDown(modifier1) && KeyState.IsKeyDown(modifier2) && KeyPressed(key);
    public bool LMouseClicked => MouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released;
    public bool RMouseClicked => MouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released;
    public bool MMouseClicked => MouseState.MiddleButton == ButtonState.Pressed && LastMouseState.MiddleButton == ButtonState.Released;
    public bool LMouseReleased => MouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed;
    public bool RMouseReleased => MouseState.RightButton == ButtonState.Released && LastMouseState.RightButton == ButtonState.Pressed;
    public bool MMouseReleased => MouseState.MiddleButton == ButtonState.Released && LastMouseState.MiddleButton == ButtonState.Pressed;
    public bool LMouseDown => MouseState.LeftButton == ButtonState.Pressed;
    public bool RMouseDown => MouseState.RightButton == ButtonState.Pressed;
    public bool MMouseDown => MouseState.MiddleButton == ButtonState.Pressed;
    public int ScrollWheelValue => MouseState.ScrollWheelValue;
    public int ScrollWheelChange => MouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue;

    // Properties
    public Game Game { get; set; }
    public SpriteBatch Batch { get; private set; }
    public List<Widget> Widgets { get; set; }
    public MouseState MouseState { get; private set; }
    public MouseState LastMouseState { get; private set; }
    public KeyboardState KeyState { get; private set; }
    public KeyboardState LastKeyState { get; private set; }
    public float Delta { get; private set; }
    public Texture2D? CircleOutline { get; private set; }
    public Texture2D? ArrowDown { get; private set; }
    public SpriteFont? Font { get; private set; }
    private bool _loaded { get; set; } = false;
    public GUI(Game game, SpriteBatch spriteBatch, SpriteFont guiFont)
    {
        Game = game;
        Widgets = [];
        Batch = spriteBatch;
        MouseState = new();
        KeyState = new();
        LastMouseState = new();
        LastKeyState = new();
        Font = guiFont;
    }
    public void Update(float deltaTime, MouseState mouseState, KeyboardState keyState)
    {
        // Not loaded
        if (!_loaded) { throw new Exception("GUI content needs to be loaded with LoadContent first."); }

        // Delta time
        Delta = deltaTime;

        // Input
        MouseState = mouseState;
        KeyState = keyState;

        // Updates
        foreach (Widget widget in Widgets) { widget.Update(); }

        // Last
        LastMouseState = MouseState;
        LastKeyState = KeyState;
    }
    public void Draw()
    {
        foreach (Widget widget in Widgets) { widget.Draw(); }
    }
    public void LoadContent(ContentManager content, string filepath = "")
    {
        string prepend = filepath == "" ? "" : filepath + Path.DirectorySeparatorChar;
        CircleOutline = content.Load<Texture2D>($"{prepend}CircleOutline");
        ArrowDown = content.Load<Texture2D>($"{prepend}ArrowDown");
        
        _loaded = true;
    }
    // Layers
    public void BringToBack(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.Remove(moving); Widgets.Insert(0, moving); } }
    public void BringToBack(Widget widget) { if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Insert(0, widget); } }
    public void BringToFont(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.Remove(moving); Widgets.Append(moving); } }
    public void BringToFront(Widget widget) { if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Append(widget); } }
    public void BringToIndex(Widget widget, int idx)
    {
        if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Insert(Math.Clamp(idx, 0, Widgets.Count - 1), widget); }
    }
    public void AddWidget(Widget widget) { Widgets.Add(widget); }
    public void AddWidgets(params Widget[] widgets) { Widgets.AddRange(widgets); }
    public void RemoveWidget(Widget widget) { Widgets.Remove(widget); }
    public void RemoveWidgets(params Widget[] widgets) { foreach (Widget widget in widgets) Widgets.Remove(widget); }

}
