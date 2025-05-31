using System.Numerics;
using static Raylib_cs.Raylib;

namespace H4NationalFocusGUI.services
{
    public class GuiService
    {
        public string Message;
        public float Timer;
        public GuiService()
        {
            Message = "";
            Timer = 0.0f;
        }

        public void Show(string message)
        {
            Message = message;
            Timer = 2.5f;
        }

        public void Update()
        {
            if (Timer > 0)
                Timer -= GetFrameTime();
        }

        public void Draw()
        {
            if (!string.IsNullOrEmpty(Message) && Timer > 0)
            {
                int fontSize = 20;
                int padding = 10;

                int screenWidth = GetScreenWidth();
                int screenHeight = GetScreenHeight();

                int textWidth = MeasureText(Message, fontSize);

                int posX = screenWidth - textWidth - padding;
                int posY = screenHeight - fontSize - padding;

                DrawText(Message, posX, posY, fontSize, Raylib_cs.Color.DarkGreen);
            }
        }

        public void ToggleOnClick(Vector2 mouse, Raylib_cs.Rectangle rect, ref bool flag)
        {
            if (IsMouseButtonPressed(Raylib_cs.MouseButton.Left) && CheckCollisionPointRec(mouse, rect))
                flag = !flag;
        }
    }
}