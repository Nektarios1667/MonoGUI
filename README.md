# MonoGUI 2.0

MonoGUI is a lightweight C# GUI library for MonoGame. Version 2.0 is a package-first library release: it no longer ships a demo executable or requires compiled `.xnb` assets.

## Features
- Supports buttons, labels, text inputs, radio buttons, popups, lists, menus, sliders, and scroll views.
- Customizable colors and styles.
- Events for clicks, selections, text updates, submissions, and value changes.
- Built-in slider and dropdown glyphs generated at runtime—no content-pipeline setup required.
- Safer widget layering and mutation during callbacks.

## Widgets
- Button
- Checkbox
- Dropdown
- HorizontalSlider
- InfoBox
- Input
- Label
- ListBox
- Popup
- VerticalSlider
- RadioButton / RadioGroup
- ProgressBar

## Installation
The source code can be directly downloaded and added to the project.  
Alternatively, the code can be packed into a NuGet package and installed in local projects.  

To do this in Visual Studio, follow the next steps:  
1. Clone the code from here into a new MonoGame project
2. Package the code into a NuGet package
3. Open the new project where MonoGUI will be installed
4. Open the settings and go to `Settings -> NuGet Package Manager -> Package Sources`
5. Click the green plus button in the top-right corner to add a new source
6. In the `Name` input enter a name such as "Local" and put the directory of the .nupkg in the `Source` field.
7. Click `Update` then close out and open the NuGet Package Manager
8. Change the source to the newly created "Local" source and click on MonoGUI when it appears
9. Select the current project and click `Install` to add it to the project

To install from the command line, follow the next steps:
1. Clone the code from here into a new MonoGame project
2. Package the code into a NuGet package
3. Open the new project where MonoGUI will be installed
4. Open the terminal for the new project and run `Install-Package MonoGUI -Source "C:\path\to\nuget\file"`

## Usage
Use the following code to set the GUI up in the Game class.
```csharp
protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);  // Spritebatch
    Gui = new GUI(this, _spriteBatch, font);  // Create new GUI and pass in Game, SpriteBatch, and a font
    Gui.LoadContent();  // Creates MonoGUI's built-in glyph textures

    // Add new widgets
    Gui.AddWidgets(
        new Button(Gui, new(50, 50), new(100, 30), Color.White, Color.Gray, Color.DarkGray,
            (Action<string>)Console.WriteLine, args: ["Click!"], text: "Button", font: font)
    );
}
```
Use the following code to update the GUI every frame.
```csharp
protected override void Update(GameTime gameTime)
{
    // Key state and mouse state
    KeyState = Keyboard.GetState();
    MouseState = Mouse.GetState();

    // Exit
    if (KeyState.IsKeyDown(Keys.Escape)) { Exit(); }

    // Time since previous frame in seconds
    DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    
    // Update GUI
    Gui.Update(DeltaTime, MouseState, KeyState);

    // Base
    base.Update(gameTime);
}
```
Use the following code to draw the GUI every frame.
```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    // Begin
    _spriteBatch.Begin();

    // Gui draw
    Gui.Draw();

    // End
    _spriteBatch.End();

    // Base
    base.Draw(gameTime);
}
```
Some widgets, such as the slider, have events that can be subscribed to.  
```csharp
var input = new Input(Gui, new(50, 100), new(240, 30), Color.Black, Color.White, Color.LightGray, font)
{
    Placeholder = "Type a name",
    MaxLength = 32,
};
input.TextChanged += value => Console.WriteLine($"Text: {value}");
input.Submitted += value => Console.WriteLine($"Submitted: {value}");
```

### Radio buttons

```csharp
var difficulty = new RadioGroup();
var easy = new RadioButton(Gui, new(50, 150), "Easy", Color.Black, Color.Black, Color.Gray, difficulty, font);
var hard = new RadioButton(Gui, new(50, 180), "Hard", Color.Black, Color.Black, Color.Gray, difficulty, font);
difficulty.SelectionChanged += choice => Console.WriteLine(choice?.Text);
difficulty.Select(easy);
Gui.AddWidgets(easy, hard);
```

### Progress bars

```csharp
var progress = new ProgressBar(Gui, new(50, 230), new(300, 26), Color.DarkGray, Color.LimeGreen,
    value: 0.65f, showPercentage: true, font: font);
progress.ValueChanged += value => Console.WriteLine($"Progress: {value:P0}");
Gui.AddWidget(progress);
```

`ProgressBar` supports left-to-right, right-to-left, bottom-to-top, and top-to-bottom fill directions.

## What's new in 2.0

- Fixed front-layering, text wrapping and truncation edge cases, slider track clicks, checkbox duplicate notifications, and scrollbar dragging.
- Rebuilt `Input` with keyboard repeat, cursor positioning, Delete/Home/End, Caps Lock support, placeholders, maximum length, and text events.
- Rebuilt `ScrollBar` and `ScrollBox` around a real viewport and proportional thumb.
- Added `Button.Clicked`, `Input.TextChanged`, `Input.Submitted`, `Input.Clear`, `ScrollBox.ClearItems`, `ScrollBox.ScrollBar`, `RadioButton` / `RadioGroup`, `ProgressBar`, and `GUI.Dispose`.
- `GUI.Widgets` is now intentionally read-only as a property; use `AddWidget`, `AddWidgets`, or its list methods to manage widgets.

## [License](https://creativecommons.org/licenses/by-nc-sa/4.0/deed.en)
Creative Commons Attribution-NonCommercial-ShareAlike (CC BY-NC-SA) license. Distributing and changing this code is allowed if you give appropriate credit, provide a link to the license, and indicate if changes were made. You may not use the material for commercial purposes. If you remix, transform, or build upon this code, you must distribute your contributions under the same license as the original. You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.
