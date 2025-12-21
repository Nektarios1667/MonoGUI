namespace MonoGUI;
public class MenuItem(Button button)
{
    public Button Button { get; } = button;
    public MouseMenu? SubMenu { get; private set; }

    // Attach a submenu 
    public void AttachSubMenu(MouseMenu subMenu)
    {
        SubMenu = subMenu;
        subMenu.Root = false; // Disable root for SubMenus
        Button.Function = () =>
        {
            SubMenu.Visible = true;
            SubMenu.Location = new Point(Button.Location.X + Button.Dimensions.X - Button.Border, Button.Location.Y);
            SubMenu.Update();
        };
        Button.Args = [];
    }

    // Recursively close
    public void CloseSubMenu()
    {
        if (SubMenu != null)
        {
            SubMenu.Visible = false;
            foreach (var item in SubMenu.MenuItems)
                item.CloseSubMenu();
        }
    }
}