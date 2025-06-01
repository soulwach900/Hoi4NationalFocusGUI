using Rectangle = Raylib_cs.Rectangle;
using static Raylib_cs.Raylib;
using System.Numerics;
using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class FocusPrerequisitesService
    {
        public readonly List<string> selectedPrerequisites = [];

        public void RenderPrerequisites(List<Focus> focuses, Vector2 mouse, Vector2 startPos)
        {
            DrawText("Prerequisites:", (int)startPos.X, (int)startPos.Y, 18, Raylib_cs.Color.White);

            var y = (int)startPos.Y + 25;

            foreach (var focus in focuses)
            {
                var box = new Rectangle(startPos.X, y, 20, 20);
                var selected = selectedPrerequisites.Contains(focus.Id);

                DrawRectangleRec(box, selected ? Raylib_cs.Color.Green : Raylib_cs.Color.White);
                DrawRectangleLinesEx(box, 1, Raylib_cs.Color.Black);
                DrawText(focus.Id, (int)startPos.X + 30, y, 16, Raylib_cs.Color.Black);

                if (IsMouseButtonPressed(Raylib_cs.MouseButton.Left) && CheckCollisionPointRec(mouse, box))
                {
                    if (selected)
                        selectedPrerequisites.Remove(focus.Id);
                    else
                        selectedPrerequisites.Add(focus.Id);
                }

                y += 30;
            }
        }
    }
}