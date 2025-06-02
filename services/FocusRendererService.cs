using static Raylib_cs.Raylib;
using System.Numerics;
using H4NationalFocusGUI.components;
using H4NationalFocusGUI.enums;
using H4NationalFocusGUI.functional;
using Raylib_cs;

namespace H4NationalFocusGUI.services
{
    public class FocusRendererService
    {
        private readonly GuiLayout layout = new();

        public void DrawTextBox(ref string input, ref ActiveTextField activeField, Rectangle fieldRect,
            ActiveTextField thisField)
        {
            var isActive = activeField == thisField;

            if (CheckCollisionPointRec(GetMousePosition(), fieldRect) && IsMouseButtonPressed(MouseButton.Left))
            {
                activeField = thisField;
            }

            if (isActive)
            {
                var key = GetCharPressed();
                while (key > 0)
                {
                    if (key is >= 32 and <= 125)
                    {
                        input += (char)key;
                    }

                    key = GetCharPressed();
                }

                if (IsKeyPressed(KeyboardKey.Backspace) && input.Length > 0)
                {
                    input = input[..^1];
                }
            }

            DrawRectangleRec(fieldRect, isActive ? Color.LightGray : Color.Gray);
            DrawText(input, (int)fieldRect.X + 5, (int)fieldRect.Y + 5, 20, Color.White);
        }

        public static void DrawField(Rectangle field, bool active, string text, Color? color = null)
        {
            Color fillColor = color ?? (active ? Color.LightGray : Color.Gray);

            DrawRectangleRec(field, fillColor);
            DrawText(text, (int)field.X + 5, (int)field.Y + 5, 20, Color.White);
        }

        public void ExplorerOpen(ref string iconInput, Dictionary<string, Texture2D> loadedIcons, ref string statusMessage, ref float statusTimer)
        {
            var selected = ExplorerService.FileSelector("Choose Focus Icon", "National Focus Icon (*.png)", ["png"]);
            if (string.IsNullOrEmpty(selected)) return;

            try
            {
                var finalIconPath = $"mod/gfx/interface/goals/{Path.GetFileNameWithoutExtension(selected).ToLower()}.dds";

                MagickConverter.ConvertPngToDds(selected, "mod/gfx/interface/goals", Path.GetFileNameWithoutExtension(selected).ToLower());

                if (!loadedIcons.ContainsKey(finalIconPath))
                {
                    var tex = LoadTexture(finalIconPath);
                    loadedIcons[finalIconPath] = tex;
                }

                iconInput = finalIconPath;

                statusMessage = "Icon loaded successfully!";
                statusTimer = 3.0f;
            }
            catch (Exception ex)
            {
                statusMessage = "Error loading icon: " + ex.Message;
                statusTimer = 3.0f;
            }
        }

        public void RenderFocuses(List<Focus> focuses, Vector2 mouse, Dictionary<string, Texture2D> loadedIcons,
            ref Focus? pendingDeleteFocus, ref Rectangle pendingRect, float scrollX, float scrollY)
        {
            var xStart = layout.FocusDisplayArea.X;
            var yStart = layout.FocusDisplayArea.Y;

            // DRAW LINES
            foreach (var focus in focuses)
            {
                foreach (var prereqId in focus.Prerequisites)
                {
                    var prereq = focuses.Find(f => f.Id == prereqId);
                    if (prereq == null) continue;

                    var start = new Vector2(
                        xStart + prereq.X * 80 - scrollX + 32,
                        yStart + prereq.Y * 80 - scrollY + 64
                    );
                    
                    var end = new Vector2(
                        xStart + focus.X * 80 - scrollX + 32,
                        yStart + focus.Y * 80 - scrollY
                    );
                    
                    DrawLineEx(start, end, 2, Color.Red);
                }
            }

            // DRAW FOCUS
            for (var i = focuses.Count - 1; i >= 0; i--)
            {
                var focus = focuses[i];
                var fx = xStart + focus.X * 80 - scrollX;
                var fy = yStart + focus.Y * 80 - scrollY;

                var rect = new Rectangle(fx, fy, 64, 64);

                // LOAD FOCUS TEXTURE
                if (!string.IsNullOrWhiteSpace(focus.IconPath) && loadedIcons.TryGetValue(focus.IconPath, out var icon))
                {
                    DrawTexture(icon, (int)fx, (int)fy, Color.White);
                    DrawText(focus.Id, (int)fx + 5, (int)fy + 48, 12, Color.White);
                }
                else
                {
                    DrawRectangleRec(rect, Color.SkyBlue);
                    DrawRectangleLines((int)fx, (int)fy, 64, 64, Color.Black);
                    DrawText(focus.Id, (int)fx + 5, (int)fy + 48, 12, Color.White);
                }

                var hovered = CheckCollisionPointRec(mouse, rect);
                if (hovered)
                {
                    DrawRectangleLinesEx(rect, 3, Color.Red);
                    if (IsMouseButtonPressed(MouseButton.Left) && pendingDeleteFocus == null)
                    {
                        pendingDeleteFocus = focus;
                        pendingRect = rect;
                    }
                }
            }
        }

        public bool? Show(Rectangle rect, Vector2 mouse, string message)
        {
            DrawRectangleRec(rect, Color.LightGray);
            DrawRectangleLinesEx(rect, 2, Color.DarkGray);
            DrawText(message, (int)rect.X + 20, (int)rect.Y + 15, 16, Color.White);

            Rectangle yesBtn = new(rect.X + 20, rect.Y + 50, 70, 30);
            Rectangle noBtn = new(rect.X + 110, rect.Y + 50, 70, 30);

            DrawRectangleRec(yesBtn, Color.Green);
            DrawText("Yes", (int)yesBtn.X + 15, (int)yesBtn.Y + 8, 16, Color.White);

            DrawRectangleRec(noBtn, Color.Red);
            DrawText("No", (int)noBtn.X + 15, (int)noBtn.Y + 8, 16, Color.White);

            if (!IsMouseButtonPressed(MouseButton.Left)) return null;
            if (CheckCollisionPointRec(mouse, yesBtn)) return true;
            if (CheckCollisionPointRec(mouse, noBtn)) return false;

            return null;
        }
    }
}