using static Raylib_cs.Raylib;
using H4NationalFocusGUI.functional;
using Color = Raylib_cs.Color;
using Gtk;
using ImageMagick;

namespace h4nationalfocusgui
{
    internal static class Program
    {
        private static readonly Gui Gui = new();

        public static void Main()
        {
            InitWindow(1280, 720, "H4NationalFocusGUI");
            SetTargetFPS(60);

            MagickNET.Initialize();
            Application.Init();
            
            FolderStructure.CreateStructure();

            while (!WindowShouldClose())
            {
                while (Application.EventsPending())
                    Application.RunIteration();
                
                Update();
                Render();
            }

            CloseWindow();
            Application.Quit();
        }

        private static void Update()
        {
            Gui.Update();
        }

        private static void Render()
        {
            BeginDrawing();
            ClearBackground(Color.RayWhite);
            Gui.Render();
            EndDrawing();
        }
    }
}