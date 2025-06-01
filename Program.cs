using H4NationalFocusGUI.functional;
using static Raylib_cs.Raylib;
using Color = Raylib_cs.Color;

namespace h4nationalfocusgui
{
    internal static class Program
    {
        private static Gui gui = new();

        public static void Main()
        {
            InitWindow(1280, 720, "H4NationalFocusGUI");
            SetTargetFPS(60);

            FolderStructure.CreateStructure();

            while (!WindowShouldClose())
            {
                Update();
                Render();
            }

            CloseWindow();
        }

        private static void Update()
        {
            gui.Update();
        }

        private static void Render()
        {
            BeginDrawing();
            ClearBackground(Color.RayWhite);
            gui.Render();
            EndDrawing();
        }
    }
}