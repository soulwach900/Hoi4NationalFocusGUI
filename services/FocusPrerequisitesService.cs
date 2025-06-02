using static Raylib_cs.Raylib;

using Rectangle = Raylib_cs.Rectangle;
using System.Numerics;
using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class FocusPrerequisitesService
    {
        public readonly List<string> SelectedPrerequisites = [];

        public void RenderPrerequisites(List<Focus> focuses, Vector2 mouse, Vector2 startPos)
        {
            DrawText("Prerequisites:", (int)startPos.X, (int)startPos.Y, 18, Raylib_cs.Color.White);

            var y = (int)startPos.Y + 25;

            foreach (var focus in focuses)
            {
                var box = new Rectangle(startPos.X, y, 20, 20);
                var selected = SelectedPrerequisites.Contains(focus.Id);

                DrawRectangleRec(box, selected ? Raylib_cs.Color.Green : Raylib_cs.Color.White);
                DrawRectangleLinesEx(box, 1, Raylib_cs.Color.Black);
                DrawText(focus.Id, (int)startPos.X + 30, y, 16, Raylib_cs.Color.Black);

                if (IsMouseButtonPressed(Raylib_cs.MouseButton.Left) && CheckCollisionPointRec(mouse, box))
                {
                    if (selected)
                        SelectedPrerequisites.Remove(focus.Id);
                    else
                        SelectedPrerequisites.Add(focus.Id);
                }

                y += 30;
            }
        }
    }
}