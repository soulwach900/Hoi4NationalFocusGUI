using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace h4nationalfocusgui
{
    public class Gui
    {
        private List<Focus> focuses = new();

        private string idInput = "", iconInput = "", nameInput = "", descInput = "";
        private string costInput = "10", xInput = "0", yInput = "0";

        private bool typingId = false, typingName = false, typingDesc = false;
        private bool typingCost = false, typingX = false, typingY = false;

        private List<string> selectedPrerequisites = new();

        private Rectangle idField = new(20, 50, 280, 30);
        private Rectangle nameField = new(20, 100, 280, 30);
        private Rectangle descField = new(20, 150, 280, 30);
        private Rectangle costField = new(20, 200, 100, 30);
        private Rectangle xField = new(20, 250, 100, 30);
        private Rectangle yField = new(20, 300, 100, 30);
        private Rectangle iconField = new(20, 350, 280, 30);
        private Rectangle saveButton = new(20, 400, 100, 30);
        private Rectangle saveYamlButton = new(140, 400, 150, 30);
        private Rectangle focusShowField;

        private string statusMessage = "";
        private float statusTimer = 0;

        private Dictionary<string, Texture2D> loadedIcons = new();

        private Focus? pendingDeleteFocus = null;
        private bool isDeleteConfirming = false;
        private Rectangle pendingRect = new();
        
        public void Update()
        {
            Vector2 mouse = GetMousePosition();

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                typingId = CheckCollisionPointRec(mouse, idField);
                typingName = CheckCollisionPointRec(mouse, nameField);
                typingDesc = CheckCollisionPointRec(mouse, descField);
                typingCost = CheckCollisionPointRec(mouse, costField);
                typingX = CheckCollisionPointRec(mouse, xField);
                typingY = CheckCollisionPointRec(mouse, yField);

                if (CheckCollisionPointRec(mouse, iconField))
                {
                    Thread thread = new(() =>
                    {
                        OpenFileDialog ofd = new()
                        {
                            Filter = "Image Files (*.dds;*.png)|*.dds;*.png",
                            Title = "Choose a focus icon"
                        };

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            string selectedPath = Path.GetFullPath(ofd.FileName);
                            iconInput = selectedPath;

                            if (!loadedIcons.ContainsKey(selectedPath))
                            {
                                try
                                {
                                    Texture2D tex = LoadTexture(selectedPath);
                                    loadedIcons[selectedPath] = tex;
                                }
                                catch (Exception ex)
                                {
                                    statusMessage = "Error loading icon: " + ex.Message;
                                    statusTimer = 3f;
                                }
                            }
                        }
                    });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }

                if (CheckCollisionPointRec(mouse, saveButton))
                    SaveFocus();

                if (CheckCollisionPointRec(mouse, saveYamlButton))
                    SaveFocusYaml();

                int y = 460;
                foreach (var focus in focuses)
                {
                    Rectangle checkbox = new(20, y, 20, 20);
                    if (CheckCollisionPointRec(mouse, checkbox))
                    {
                        if (selectedPrerequisites.Contains(focus.Id))
                            selectedPrerequisites.Remove(focus.Id);
                        else
                            selectedPrerequisites.Add(focus.Id);
                    }

                    y += 30;
                }
            }

            int key = GetCharPressed();
            while (key > 0)
            {
                char c = (char)key;

                if (typingId) idInput += c;
                else if (typingName) nameInput += c;
                else if (typingDesc) descInput += c;
                else if (typingCost && char.IsDigit(c)) costInput += c;
                else if (typingX && char.IsDigit(c)) xInput += c;
                else if (typingY && char.IsDigit(c)) yInput += c;

                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace))
            {
                if (typingId && idInput.Length > 0) idInput = idInput[..^1];
                else if (typingName && nameInput.Length > 0) nameInput = nameInput[..^1];
                else if (typingDesc && descInput.Length > 0) descInput = descInput[..^1];
                else if (typingCost && costInput.Length > 0) costInput = costInput[..^1];
                else if (typingX && xInput.Length > 0) xInput = xInput[..^1];
                else if (typingY && yInput.Length > 0) yInput = yInput[..^1];
            }

            if (statusTimer > 0)
                statusTimer -= GetFrameTime();
        }

        public void Render()
        {
            DrawRectangle(0, 0, 320, 720, Color.DarkGray);
            DrawText("Focus Creator", 20, 10, 21, Color.Blue);

            DrawField(idField, typingId, $"ID: {idInput}");
            DrawField(nameField, typingName, $"Name: {nameInput}");
            DrawField(descField, typingDesc, $"Desc: {descInput}");
            DrawField(costField, typingCost, $"Cost: {costInput}");
            DrawField(xField, typingX, $"X: {xInput}");
            DrawField(yField, typingY, $"Y: {yInput}");
            DrawField(iconField, false, $"Icon: {Path.GetFileName(iconInput)}");

            DrawRectangleRec(saveButton, Color.Green);
            DrawText("Save", 30, 405, 20, Color.Black);

            DrawRectangleRec(saveYamlButton, Color.DarkGreen);
            DrawText("Save Yaml", 145, 405, 20, Color.Black);

            DrawText("Prerequisites:", 20, 440, 18, Color.White);
            int y = 460;
            foreach (var focus in focuses)
            {
                bool selected = selectedPrerequisites.Contains(focus.Id);
                DrawRectangle(20, y, 20, 20, selected ? Color.Green : Color.White);
                DrawRectangleLines(20, y, 20, 20, Color.Black);
                DrawText(focus.Id, 50, y, 16, Color.Black);
                y += 30;
            }

            if (!string.IsNullOrEmpty(statusMessage) && statusTimer > 0)
                DrawText(statusMessage, 320 + 20, 20, 20, Color.DarkGreen);

            int xStart = 340, yStart = 50;

            foreach (var focus in focuses)
            {
                foreach (string prereqId in focus.Prerequisites)
                {
                    var prereq = focuses.Find(f => f.Id == prereqId);
                    if (prereq != null)
                    {
                        Vector2 start = new(xStart + prereq.X * 80 + 32, yStart + prereq.Y * 80 + 64);
                        Vector2 end = new(xStart + focus.X * 80 + 32, yStart + focus.Y * 80);
                        DrawLineEx(start, end, 2, Color.Red);
                    }
                }
            }
            
            Vector2 mouse = Raylib.GetMousePosition();
            
            for (int i = focuses.Count - 1; i >= 0; i--)
            {
                var focus = focuses[i];
                int fx = xStart + focus.X * 80;
                int fy = yStart + focus.Y * 80;

                Rectangle rect = new Rectangle(fx, fy, 64, 64);

                Raylib.DrawRectangleRec(rect, Color.SkyBlue);

                if (!string.IsNullOrWhiteSpace(focus.Icon) && loadedIcons.ContainsKey(focus.Icon))
                {
                    Raylib.DrawTexture(loadedIcons[focus.Icon], fx, fy, Color.White);
                }

                bool hovered = Raylib.CheckCollisionPointRec(mouse, rect);

                if (hovered)
                {
                    Raylib.DrawRectangleLinesEx(rect, 3, Color.Red);
                    
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left) && pendingDeleteFocus == null)
                    {
                        pendingDeleteFocus = focus;
                        pendingRect = rect;
                        isDeleteConfirming = true;
                    }
                }

                Raylib.DrawRectangleLines(fx, fy, 64, 64, Color.Black);
                Raylib.DrawText(focus.Id, fx + 5, fy + 48, 12, Color.Black);
            }
            
            if (pendingDeleteFocus != null && isDeleteConfirming)   
            {
                int screenWidth = Raylib.GetScreenWidth();
                int screenHeight = Raylib.GetScreenHeight();

                int boxWidth = (int)(screenWidth * 0.9f);
                int boxHeight = (int)(screenHeight * 0.9f);

                int boxX = (screenWidth - boxWidth) / 2;
                int boxY = (screenHeight - boxHeight) / 2;

                Rectangle confirmBox = new Rectangle(boxX, boxY, boxWidth, boxHeight);

                Raylib.DrawRectangleRec(confirmBox, Color.LightGray);
                Raylib.DrawRectangleLinesEx(confirmBox, 2, Color.DarkGray);

                Raylib.DrawText("Delete focus?", boxX + 20, boxY + 15, 16, Color.Black);
                
                Rectangle yesBtn = new Rectangle(boxX + 20, boxY + 50, 70, 30);
                Rectangle noBtn = new Rectangle(boxX + 110, boxY + 50, 70, 30);

                Raylib.DrawRectangleRec(yesBtn, Color.Green);
                Raylib.DrawText("Yes", (int)yesBtn.X + 15, (int)yesBtn.Y + 8, 16, Color.Black);

                Raylib.DrawRectangleRec(noBtn, Color.Red);
                Raylib.DrawText("No", (int)noBtn.X + 15, (int)noBtn.Y + 8, 16, Color.Black);
                
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    if (Raylib.CheckCollisionPointRec(mouse, yesBtn))
                    {
                        focuses.Remove(pendingDeleteFocus);
                        pendingDeleteFocus = null;
                        isDeleteConfirming = false;
                    }
                    else if (Raylib.CheckCollisionPointRec(mouse, noBtn))
                    {
                        pendingDeleteFocus = null;
                        isDeleteConfirming = false;
                    }
                }
            }
        }

        private void DrawField(Rectangle field, bool active, string text)
        {
            DrawRectangleRec(field, active ? Color.LightGray : Color.Gray);
            DrawText(text, (int)field.X + 5, (int)field.Y + 5, 20, Color.Black);
        }

        private void SaveFocus()
        {
            if (string.IsNullOrWhiteSpace(idInput)) return;

            int.TryParse(xInput, out int xVal);
            int.TryParse(yInput, out int yVal);
            int.TryParse(costInput, out int costVal);

            var newFocus = new Focus(idInput, iconInput, nameInput, descInput, xVal, yVal, costVal);
            newFocus.Prerequisites.AddRange(selectedPrerequisites);

            focuses.Add(newFocus);
            statusMessage = "Focus saved successfully!";
            statusTimer = 2.5f;

            idInput = nameInput = descInput = iconInput = "";
            xInput = yInput = "0";
            costInput = "10";
            selectedPrerequisites.Clear();
        }

        private void SaveFocusYaml()
        {
            if (focuses.Count == 0)
            {
                statusMessage = "No focus created to save!";
                statusTimer = 2.5f;
                return;
            }

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fullPath = Path.Combine(desktopPath, $"focus_{focuses[0].Id}_tree.txt");
                focuses[0].GenerateFocusTreeFile(focuses, fullPath);
                statusMessage = $"File saved to desktop!";
            }
            catch (Exception ex)
            {
                statusMessage = $"Erro: {ex.Message}";
            }

            statusTimer = 3f;
        }
    }
}