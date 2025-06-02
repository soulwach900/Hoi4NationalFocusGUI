using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;

namespace H4NationalFocusGUI.components
{
    public class GuiLayout
    {
        private readonly Vector2 screen = new(GetScreenWidth(), GetScreenHeight());
        
        public Rectangle CreateMenuPanel = new(310, 0, 320, 720);
        public Rectangle CreateMenuPanelPrerequisites => new(CreateMenuPanel.X + CreateMenuPanel.Width, CreateMenuPanel.Y, 320, 720);

        public Rectangle SaveYamlButton = new(20, 400, 150, 30);
        public Rectangle LoadFocusButton = new(20, 440, 150, 30);

        public Rectangle CreateFocusField = new(20, 10, 280, 30);
        public Rectangle CountryNameField = new(20, 10, 280, 30);

        public Rectangle IdField = new(20, 50, 280, 30);
        public Rectangle NameField = new(20, 100, 280, 30);
        public Rectangle DescField = new(20, 150, 280, 30);
        public Rectangle CostField = new(20, 200, 100, 30);
        public Rectangle XField = new(20, 250, 100, 30);
        public Rectangle YField = new(20, 300, 100, 30);

        public Rectangle CreateIdField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 20, 280, 30);
        public Rectangle CreateNameField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 60, 280, 30);
        public Rectangle CreateDescField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 100, 280, 30);
        public Rectangle CreateCostField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 140, 100, 30);
        public Rectangle CreateXField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 180, 100, 30);
        public Rectangle CreateYField => new(CreateMenuPanel.X + 140, CreateMenuPanel.Y + 180, 100, 30);
        public Rectangle CreateIconField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 220, 280, 30);
        public Rectangle CreateSaveButton => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 300, 100, 30);
        public Rectangle CreatePrereqFocusField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 260, 280, 30);

        public Rectangle PendingRect = new(400, 300, 200, 100);
        
        public Rectangle FocusDisplayArea => new(CreateMenuPanel.X + 10, CreateMenuPanel.Y + 10, 1, 1);
        
        public Rectangle FocusDisplayScrollVertical => new(screen.X - 10, 0, 1, 1);
        public Rectangle FocusDisplayScrollHorizontal => new(CreateMenuPanel.X + 10, screen.Y - 10, 1, 1);
    }
}