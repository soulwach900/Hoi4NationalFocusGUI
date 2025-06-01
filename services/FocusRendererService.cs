using System.Numerics;
using H4NationalFocusGUI.components;
using H4NationalFocusGUI.enums;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace H4NationalFocusGUI.services
{
    public class FocusRendererService
    {
        private readonly GuiLayout layout = new();
        private GuiService guiService = new();

        public void DrawTextBox(ref string input, ref ActiveTextField activeField, Raylib_cs.Rectangle fieldRect,
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

            DrawRectangleRec(fieldRect, isActive ? Raylib_cs.Color.LightGray : Raylib_cs.Color.Gray);
            DrawText(input, (int)fieldRect.X + 5, (int)fieldRect.Y + 5, 20, Raylib_cs.Color.White);
        }

        public void DrawField(Raylib_cs.Rectangle field, bool active, string text, Raylib_cs.Color? color = null)
        {
            Raylib_cs.Color fillColor = color ?? (active ? Raylib_cs.Color.LightGray : Raylib_cs.Color.Gray);

            DrawRectangleRec(field, fillColor);
            DrawText(text, (int)field.X + 5, (int)field.Y + 5, 20, Raylib_cs.Color.White);
        }

        public void WindowsExplorerOpen(
            Vector2 mouse,
            Raylib_cs.Rectangle iconField,
            ref string iconInput,
            Dictionary<string, Texture2D> loadedIcons,
            ref string statusMessage,
            ref float statusTimer)
        {
            if (!CheckCollisionPointRec(mouse, iconField)) return;
            string? selectedPath = null;

            Thread thread = new(() =>
            {
                OpenFileDialog ofd = new()
                {
                    Filter = "Image Files (*.png;)|*.png;",
                    Title = "Choose a focus icon"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = Path.GetFullPath(ofd.FileName);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (string.IsNullOrEmpty(selectedPath)) return;
            try
            {
                var finalIconPath =
                    $"mod/gfx/interface/goals/{Path.GetFileNameWithoutExtension(selectedPath).ToLower()}.dds";

                if (!loadedIcons.ContainsKey(finalIconPath))
                {
                    var tex = LoadTexture(selectedPath);
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
            ref Focus? pendingDeleteFocus, ref Raylib_cs.Rectangle pendingRect, float scrollX, float scrollY)
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
                    DrawLineEx(start, end, 2, Raylib_cs.Color.Red);
                }
            }

            // DRAW FOCUS
            for (var i = focuses.Count - 1; i >= 0; i--)
            {
                var focus = focuses[i];
                var fx = xStart + focus.X * 80 - scrollX;
                var fy = yStart + focus.Y * 80 - scrollY;

                var rect = new Raylib_cs.Rectangle(fx, fy, 64, 64);
                DrawRectangleRec(rect, Raylib_cs.Color.SkyBlue);

                if (!string.IsNullOrWhiteSpace(focus.Icon) && loadedIcons.TryGetValue(focus.Icon, out var icon))
                {
                    DrawTexture(icon, (int)fx, (int)fy, Raylib_cs.Color.White);
                }

                var hovered = CheckCollisionPointRec(mouse, rect);
                if (hovered)
                {
                    DrawRectangleLinesEx(rect, 3, Raylib_cs.Color.Red);
                    if (IsMouseButtonPressed(MouseButton.Left) && pendingDeleteFocus == null)
                    {
                        pendingDeleteFocus = focus;
                        pendingRect = rect;
                    }
                }

                DrawRectangleLines((int)fx, (int)fy, 64, 64, Raylib_cs.Color.Black);
                DrawText(focus.Id, (int)fx + 5, (int)fy + 48, 12, Raylib_cs.Color.White);
            }
        }

        public bool? Show(Raylib_cs.Rectangle rect, Vector2 mouse, string message)
        {
            DrawRectangleRec(rect, Raylib_cs.Color.LightGray);
            DrawRectangleLinesEx(rect, 2, Raylib_cs.Color.DarkGray);
            DrawText(message, (int)rect.X + 20, (int)rect.Y + 15, 16, Raylib_cs.Color.White);

            Raylib_cs.Rectangle yesBtn = new(rect.X + 20, rect.Y + 50, 70, 30);
            Raylib_cs.Rectangle noBtn = new(rect.X + 110, rect.Y + 50, 70, 30);

            DrawRectangleRec(yesBtn, Raylib_cs.Color.Green);
            DrawText("Yes", (int)yesBtn.X + 15, (int)yesBtn.Y + 8, 16, Raylib_cs.Color.White);

            DrawRectangleRec(noBtn, Raylib_cs.Color.Red);
            DrawText("No", (int)noBtn.X + 15, (int)noBtn.Y + 8, 16, Raylib_cs.Color.White);

            if (!IsMouseButtonPressed(MouseButton.Left)) return null;
            if (CheckCollisionPointRec(mouse, yesBtn)) return true;
            if (CheckCollisionPointRec(mouse, noBtn)) return false;

            return null;
        }
    }
}