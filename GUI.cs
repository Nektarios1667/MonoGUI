global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Content;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
global using MonoGame.Extended;
global using MonoGUI.Widgets;
global using System;
global using System.Collections.Generic;
using System.IO;


namespace MonoGUI;

public enum TextAlign
{
    Left,
    Middle,
    Right,
}
public sealed class GUI : IDisposable
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
    public Game Game { get; }
    public SpriteBatch Batch { get; }
    public List<Widget> Widgets { get; } = [];
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
        Game = game ?? throw new ArgumentNullException(nameof(game));
        Batch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
        MouseState = new();
        KeyState = new();
        LastMouseState = new();
        LastKeyState = new();
        Font = guiFont ?? throw new ArgumentNullException(nameof(guiFont));
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
        // Copying permits click handlers to add or remove widgets safely.
        foreach (Widget widget in Widgets.ToArray())
            if (widget.Enabled)
                widget.Update();

        // Last
        LastMouseState = MouseState;
        LastKeyState = KeyState;
    }
    public void Draw()
    {
        foreach (Widget widget in Widgets.ToArray()) { widget.Draw(); }
    }
    /// <summary>Creates the small built-in textures used by sliders and dropdowns.</summary>
    public void LoadContent()
    {
        CircleOutline?.Dispose();
        ArrowDown?.Dispose();
        CircleOutline = CreateCircleOutline();
        ArrowDown = CreateArrowDown();
        _loaded = true;
    }

    /// <summary>
    /// Compatibility overload. Content is no longer required because MonoGUI generates its glyph textures at runtime.
    /// </summary>
    [Obsolete("MonoGUI 2.0 does not require compiled content. Use LoadContent() instead.")]
    public void LoadContent(ContentManager content, string filepath = "")
    {
        ArgumentNullException.ThrowIfNull(content);
        LoadContent();
    }
    // Layers
    public void BringToBack(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.Remove(moving); Widgets.Insert(0, moving); } }
    public void BringToBack(Widget widget) { if (Widgets.Contains(widget)) { Widgets.Remove(widget); Widgets.Insert(0, widget); } }
    public void BringToFront(int idx) { if (idx >= 0 && idx < Widgets.Count) { Widget moving = Widgets[idx]; Widgets.RemoveAt(idx); Widgets.Add(moving); } }
    [Obsolete("Use BringToFront instead.")]
    public void BringToFont(int idx) => BringToFront(idx);
    public void BringToFront(Widget widget) { if (Widgets.Remove(widget)) { Widgets.Add(widget); } }
    public void BringToIndex(Widget widget, int idx)
    {
        if (!Widgets.Remove(widget)) return;
        Widgets.Insert(Math.Clamp(idx, 0, Widgets.Count), widget);
    }
    public void AddWidget(Widget widget) { ArgumentNullException.ThrowIfNull(widget); Widgets.Add(widget); }
    public void AddWidgets(params Widget[] widgets) { ArgumentNullException.ThrowIfNull(widgets); foreach (Widget widget in widgets) AddWidget(widget); }
    public void RemoveWidget(Widget widget) { Widgets.Remove(widget); }
    public void RemoveWidgets(params Widget[] widgets) { foreach (Widget widget in widgets) Widgets.Remove(widget); }

    public void Dispose()
    {
        CircleOutline?.Dispose();
        ArrowDown?.Dispose();
        CircleOutline = null;
        ArrowDown = null;
        _loaded = false;
    }

    private Texture2D CreateCircleOutline()
    {
        const int size = 25;
        const float center = (size - 1) / 2f;
        const float outerRadius = 11.5f;
        const float innerRadius = 0f;
        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center));
            pixels[y * size + x] = distance is <= outerRadius and >= innerRadius ? Color.White : Color.Transparent;
        }
        Texture2D texture = new(Batch.GraphicsDevice, size, size);
        texture.SetData(pixels);
        return texture;
    }

    private Texture2D CreateArrowDown()
    {
        const int width = 16;
        const int height = 10;
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            int halfWidth = Math.Max(1, (y + 1) * width / (height * 2));
            int start = width / 2 - halfWidth;
            int end = width / 2 + halfWidth;
            for (int x = start; x <= end && x < width; x++) pixels[y * width + x] = Color.White;
        }
        Texture2D texture = new(Batch.GraphicsDevice, width, height);
        texture.SetData(pixels);
        return texture;
    }

}
