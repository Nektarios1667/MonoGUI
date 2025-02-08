# MonoGUI
MonoGUI is a C# library designed for creating simple graphical user interfaces made of widgets in the MonoGame framework.

## Features
- Supports buttons, labels, text inputs, popups, and more.
- Customizable colors and styles.
- Simple event handling for user interactions.
- Lightweight and efficient for performance.

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
    Gui = new GUI(this, _spriteBatch);  // Create new GUI and pass in Game and SpriteBatch
    Gui.LoadContent(Content);  // Load content for the GUI or items may not be rendered

    // Add new widgets
    Gui.Widgets = new List<Widget> [
        new Button(Gui, new(50, 50), new(100, 30), Color.White, Color.Gray, Color.DarkGray, (Action<string>)Console.WriteLine, args: ["Click!"], text: $"Button", font: Arial)
        // Add new widgets here
    ];
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
HorizontalSlider slider = new HorizontalSlider(Gui, new(100, 725), 100, Color.Black, new(55, 55, 55));
slider.ValueChanged += Console.WriteLine;
```

## [License](https://creativecommons.org/licenses/by-nc-sa/4.0/deed.en)
Creative Commons Attribution-NonCommercial-ShareAlike (CC BY-NC-SA) license. Distributing and changing this code is allowed if you give appropriate credit, provide a link to the license, and indicate if changes were made. You may not use the material for commercial purposes. If you remix, transform, or build upon this code, you must distribute your contributions under the same license as the original. You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.
