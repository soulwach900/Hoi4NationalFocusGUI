using H4NationalFocusGUI.functional;
using static Raylib_cs.Raylib;
using Color = Raylib_cs.Color;

namespace h4nationalfocusgui
{
    internal class Program
    {
        private static Gui gui = new Gui();
        private static FolderStructury structury = new FolderStructury();

        public static void Main(string[] args)
        {
            InitWindow(1280, 720, "H4NationalFocusGUI");
            SetTargetFPS(60);

            structury.CreateStructury();

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