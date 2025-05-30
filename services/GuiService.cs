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
                DrawText(Message, 340, 20, 20, Raylib_cs.Color.DarkGreen);
        }
    }
}