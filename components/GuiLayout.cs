using System.Numerics;
using static Raylib_cs.Raylib;

namespace H4NationalFocusGUI.components
{
    public class GuiLayout
    {
        private readonly Vector2 screen = new(GetScreenWidth(), GetScreenHeight());
        
        public Raylib_cs.Rectangle CreateMenuPanel = new(310, 0, 320, 720);
        public Raylib_cs.Rectangle CreateMenuPanelPrerequisites => new(CreateMenuPanel.X + CreateMenuPanel.Width, CreateMenuPanel.Y, 320, 720);

        public Raylib_cs.Rectangle SaveYamlButton = new(20, 400, 150, 30);
        public Raylib_cs.Rectangle LoadFocusButton = new(20, 440, 150, 30);

        public Raylib_cs.Rectangle CreateFocusField = new(20, 10, 280, 30);
        public Raylib_cs.Rectangle CountryNameField = new(20, 10, 280, 30);

        public Raylib_cs.Rectangle IdField = new(20, 50, 280, 30);
        public Raylib_cs.Rectangle NameField = new(20, 100, 280, 30);
        public Raylib_cs.Rectangle DescField = new(20, 150, 280, 30);
        public Raylib_cs.Rectangle CostField = new(20, 200, 100, 30);
        public Raylib_cs.Rectangle XField = new(20, 250, 100, 30);
        public Raylib_cs.Rectangle YField = new(20, 300, 100, 30);

        public Raylib_cs.Rectangle CreateIdField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 20, 280, 30);
        public Raylib_cs.Rectangle CreateNameField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 60, 280, 30);
        public Raylib_cs.Rectangle CreateDescField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 100, 280, 30);
        public Raylib_cs.Rectangle CreateCostField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 140, 100, 30);
        public Raylib_cs.Rectangle CreateXField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 180, 100, 30);
        public Raylib_cs.Rectangle CreateYField => new(CreateMenuPanel.X + 140, CreateMenuPanel.Y + 180, 100, 30);
        public Raylib_cs.Rectangle CreateIconField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 220, 280, 30);
        public Raylib_cs.Rectangle CreateSaveButton => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 300, 100, 30);
        public Raylib_cs.Rectangle CreatePrereqFocusField => new(CreateMenuPanel.X + 20, CreateMenuPanel.Y + 260, 280, 30);

        public Raylib_cs.Rectangle PendingRect = new(400, 300, 200, 100);
        
        public Raylib_cs.Rectangle FocusDisplayArea => new(CreateMenuPanel.X + 20, 15, 1, 1);
        
        public Raylib_cs.Rectangle FocusDisplayScrollVertical => new(screen.X - 10, 0, 1, 1);
        public Raylib_cs.Rectangle FocusDisplayScrollHorizontal => new(CreateMenuPanel.X + 10, screen.Y - 10, 1, 1);
    }
}