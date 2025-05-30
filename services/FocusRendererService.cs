using System.Numerics;
using H4NationalFocusGUI.components;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace H4NationalFocusGUI.services
{
    public class FocusRendererService
    {
        public void DrawTextBox(ref string inputText, ref bool typing, Raylib_cs.Rectangle box, int maxLength = 20)
        {
            Vector2 mouse = GetMousePosition();
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                typing = CheckCollisionPointRec(mouse, box);
            }
            if (typing)
            {
                int key = GetCharPressed();
                while (key > 0)
                {
                    if (key >= 32 && key <= 125 && inputText.Length < maxLength)
                    {
                        inputText += (char)key;
                    }
                    key = GetCharPressed();
                }
                if (IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
                {
                    inputText = inputText.Substring(0, inputText.Length - 1);
                }
            }
            DrawRectangleRec(box, typing ? Raylib_cs.Color.LightGray : Raylib_cs.Color.Gray);
            DrawRectangleLinesEx(box, 1, Raylib_cs.Color.Black);
            DrawText(inputText, (int)box.X + 5, (int)box.Y + 5, 20, Raylib_cs.Color.Black);
        }

        public void DrawField(Raylib_cs.Rectangle field, bool active, string text)
        {
            DrawRectangleRec(field, active ? Raylib_cs.Color.LightGray : Raylib_cs.Color.Gray);
            DrawText(text, (int)field.X + 5, (int)field.Y + 5, 20, Raylib_cs.Color.Black);
        }

        public bool WindowsExplorerOpen(Vector2 mouse,
    Raylib_cs.Rectangle iconField,
    ref string iconInput,
    Dictionary<string, Texture2D> loadedIcons,
    ref string statusMessage,
    ref float statusTimer)
        {
            if (CheckCollisionPointRec(mouse, iconField))
            {
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

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    try
                    {
                        if (!loadedIcons.ContainsKey(selectedPath))
                        {
                            Texture2D tex = LoadTexture(selectedPath);
                            loadedIcons[selectedPath] = tex;
                        }

                        iconInput = selectedPath;
                        statusMessage = "Icon loaded successfully!";
                        statusTimer = 3.0f;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        statusMessage = "Error loading icon: " + ex.Message;
                        statusTimer = 3.0f;
                        return false;
                    }
                }
            }

            return false;
        }

        public void RenderFocuses(List<Focus> focuses, Vector2 mouse, Dictionary<string, Texture2D> loadedIcons,
                               ref Focus? pendingDeleteFocus, ref Raylib_cs.Rectangle pendingRect)
        {
            int xStart = 340, yStart = 50;

            // Draw lines
            foreach (var focus in focuses)
            {
                foreach (var prereqId in focus.Prerequisites)
                {
                    var prereq = focuses.Find(f => f.Id == prereqId);
                    if (prereq != null)
                    {
                        Vector2 start = new(xStart + prereq.X * 80 + 32, yStart + prereq.Y * 80 + 64);
                        Vector2 end = new(xStart + focus.X * 80 + 32, yStart + focus.Y * 80);
                        DrawLineEx(start, end, 2, Raylib_cs.Color.Red);
                    }
                }
            }

            // Draw focus icons
            for (int i = focuses.Count - 1; i >= 0; i--)
            {
                var focus = focuses[i];
                int fx = xStart + focus.X * 80;
                int fy = yStart + focus.Y * 80;

                Raylib_cs.Rectangle rect = new Raylib_cs.Rectangle(fx, fy, 64, 64);
                DrawRectangleRec(rect, Raylib_cs.Color.SkyBlue);

                if (!string.IsNullOrWhiteSpace(focus.Icon) && loadedIcons.ContainsKey(focus.Icon))
                {
                    DrawTexture(loadedIcons[focus.Icon], fx, fy, Raylib_cs.Color.White);
                }

                bool hovered = CheckCollisionPointRec(mouse, rect);
                if (hovered)
                {
                    DrawRectangleLinesEx(rect, 3, Raylib_cs.Color.Red);
                    if (IsMouseButtonPressed(MouseButton.Left) && pendingDeleteFocus == null)
                    {
                        pendingDeleteFocus = focus;
                        pendingRect = rect;
                    }
                }

                DrawRectangleLines(fx, fy, 64, 64, Raylib_cs.Color.Black);
                DrawText(focus.Id, fx + 5, fy + 48, 12, Raylib_cs.Color.Black);
            }
        }

        public bool? Show(Raylib_cs.Rectangle rect, Vector2 mouse, string message)
        {
            DrawRectangleRec(rect, Raylib_cs.Color.LightGray);
            DrawRectangleLinesEx(rect, 2, Raylib_cs.Color.DarkGray);
            DrawText(message, (int)rect.X + 20, (int)rect.Y + 15, 16, Raylib_cs.Color.Black);

            Raylib_cs.Rectangle yesBtn = new(rect.X + 20, rect.Y + 50, 70, 30);
            Raylib_cs.Rectangle noBtn = new(rect.X + 110, rect.Y + 50, 70, 30);

            DrawRectangleRec(yesBtn, Raylib_cs.Color.Green);
            DrawText("Yes", (int)yesBtn.X + 15, (int)yesBtn.Y + 8, 16, Raylib_cs.Color.Black);

            DrawRectangleRec(noBtn, Raylib_cs.Color.Red);
            DrawText("No", (int)noBtn.X + 15, (int)noBtn.Y + 8, 16, Raylib_cs.Color.Black);

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                if (CheckCollisionPointRec(mouse, yesBtn)) return true;
                if (CheckCollisionPointRec(mouse, noBtn)) return false;
            }

            return null;
        }
    }
}