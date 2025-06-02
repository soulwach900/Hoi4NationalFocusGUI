using static Raylib_cs.Raylib;
using System.Numerics;
using H4NationalFocusGUI.components;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace H4NationalFocusGUI.services
{
    public class GuiService
    {
        private readonly GuiLayout layout = new();
        
        private float timer;

        // VERTICAL
        private float scrollY;
        private float contentHeight = 1280f;
        private readonly float viewHeight = 720f;
        private float ScrollbarHeight => viewHeight;
        private float ThumbHeight => MathF.Max(30f, (viewHeight / contentHeight) * ScrollbarHeight);
        private float ThumbY => (scrollY / Math.Max(1, contentHeight - viewHeight)) * (ScrollbarHeight - ThumbHeight);
        private bool draggingScroll;
        private float dragOffsetY;

        // HORIZONTAL
        private float scrollX;
        private float contentWidth = 3000f;
        private readonly float viewWidth = 1280f;
        private float ScrollbarWidth => viewWidth;
        private float ThumbWidth => MathF.Max(30f, (viewWidth / contentWidth) * ScrollbarWidth);
        private float ThumbX => (scrollX / Math.Max(1, contentWidth - viewWidth)) * (ScrollbarWidth - ThumbWidth);
        private bool draggingScrollH;
        private float dragOffsetX;

        public void Update()
        {
            if (timer > 0)
                timer -= GetFrameTime();

            UpdateScroll();
        }

        public void Draw(string message)
        {
            if (string.IsNullOrEmpty(message) || !(timer > 0)) return;
            const int fontSize = 20;
            const int padding = 10;

            var screenWidth = GetScreenWidth();
            var screenHeight = GetScreenHeight();

            var textWidth = MeasureText(message, fontSize);

            var posX = screenWidth - textWidth - padding;
            var posY = screenHeight - fontSize - padding;

            DrawText(message, posX, posY, fontSize, Color.DarkGreen);
        }

        public void ToggleOnClick(Vector2 mouse, Rectangle rect, ref bool flag)
        {
            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, rect))
                flag = !flag;
        }

        private void UpdateScroll()
        {
            var mouse = GetMousePosition();

            // VERTICAL
            var thumbRectV = new Rectangle(
                layout.FocusDisplayScrollVertical.X,
                layout.FocusDisplayScrollVertical.Y + ThumbY,
                15,
                ThumbHeight
            );

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, thumbRectV))
            {
                draggingScroll = true;
                dragOffsetY = mouse.Y - thumbRectV.Y;
            }
            else if (IsMouseButtonReleased(MouseButton.Left))
            {
                draggingScroll = false;
            }

            if (draggingScroll)
            {
                float newThumbY = mouse.Y - dragOffsetY - layout.FocusDisplayScrollVertical.Y;
                newThumbY = Math.Clamp(newThumbY, 0, ScrollbarHeight - ThumbHeight);
                scrollY = (newThumbY / Math.Max(1, ScrollbarHeight - ThumbHeight)) *
                          Math.Max(0, contentHeight - viewHeight);
            }
            
            scrollY += GetMouseWheelMove() * 40f;
            scrollY = Math.Clamp(scrollY, 0, Math.Max(0, contentHeight - viewHeight));

            // HORIZONTAL
            var thumbRectH = new Rectangle(
                layout.FocusDisplayScrollHorizontal.X + ThumbX,
                layout.FocusDisplayScrollHorizontal.Y,
                ThumbWidth,
                15
            );

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, thumbRectH))
            {
                draggingScrollH = true;
                dragOffsetX = mouse.X - thumbRectH.X;
            }
            else if (IsMouseButtonReleased(MouseButton.Left))
            {
                draggingScrollH = false;
            }

            if (draggingScrollH)
            {
                float newThumbX = mouse.X - dragOffsetX - layout.FocusDisplayScrollHorizontal.X;
                newThumbX = Math.Clamp(newThumbX, 0, ScrollbarWidth - ThumbWidth);
                scrollX = (newThumbX / Math.Max(1, ScrollbarWidth - ThumbWidth)) *
                          Math.Max(0, contentWidth - viewWidth);
            }

            // SCROLL with --> our <--
            if (IsKeyDown(KeyboardKey.Right)) scrollX += 10f;
            if (IsKeyDown(KeyboardKey.Left)) scrollX -= 10f;
            scrollX = Math.Clamp(scrollX, 0, Math.Max(0, contentWidth - viewWidth));
        }


        public void UpdateContentSize(List<Focus> focuses)
        {
            if (focuses.Count == 0)
            {
                contentWidth = viewWidth;
                contentHeight = viewHeight;
                scrollX = 0;
                scrollY = 0;
                return;
            }

            var maxX = focuses.Select(f => f.X).DefaultIfEmpty(0).Max();
            var maxY = focuses.Select(f => f.Y).DefaultIfEmpty(0).Max();

            contentWidth = Math.Max((maxX + 1) * 80, viewWidth);
            contentHeight = Math.Max((maxY + 1) * 80, viewHeight);

            scrollX = Math.Clamp(scrollX, 0, Math.Max(0, contentWidth - viewWidth));
            scrollY = Math.Clamp(scrollY, 0, Math.Max(0, contentHeight - viewHeight));
        }

        public void DrawScrollbars()
        {
            // VERTICAL
            DrawRectangle(
                (int)layout.FocusDisplayScrollVertical.X,
                (int)layout.FocusDisplayScrollVertical.Y,
                15,
                (int)ScrollbarHeight,
                Color.Gray);

            DrawRectangle(
                (int)layout.FocusDisplayScrollVertical.X,
                (int)(layout.FocusDisplayScrollVertical.Y + ThumbY),
                15,
                (int)ThumbHeight,
                Color.DarkGray);

            // HORIZONTAL
            DrawRectangle(
                (int)layout.FocusDisplayScrollHorizontal.X,
                (int)layout.FocusDisplayScrollHorizontal.Y,
                (int)ScrollbarWidth,
                15,
                Color.Gray);

            DrawRectangle(
                (int)(layout.FocusDisplayScrollHorizontal.X + ThumbX),
                (int)layout.FocusDisplayScrollHorizontal.Y,
                (int)ThumbWidth,
                15,
                Color.DarkGray);
        }

        public float ScrollY => scrollY;
        public float ScrollX => scrollX;
    }
}