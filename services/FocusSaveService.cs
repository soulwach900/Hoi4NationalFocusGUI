using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class FocusSaveService
    {
        public (bool Success, string Message) TryAddFocus(
            string id, string name, string desc,
            string x, string y, string cost, string iconPath,
            List<string> selectedPrerequisites,
            List<Focus> focuses)
        {
            if (string.IsNullOrWhiteSpace(id))
                return (false, "Focus ID is required.");

            id = id.Replace(" ", "_").ToLower();

            if (!int.TryParse(x, out var xVal) ||
                !int.TryParse(y, out var yVal) ||
                !int.TryParse(cost, out var costVal))
            {
                return (false, "Invalid numeric values for X, Y, or cost.");
            }

            var iconId = "GFX_goal_" + id;

            var focus = new Focus(id, iconId, name, desc, xVal, yVal, costVal)
            {
                IconId = iconId,
                IconPath = iconPath
            };

            focus.Prerequisites.AddRange(selectedPrerequisites);
            focuses.Add(focus);

            return (true, "Focus saved successfully.");
        }

        public void SaveFocus(List<Focus> focuses)
        {
            if (focuses.Count == 0) return;

            try
            {
                string fullPath = Path.Combine("mod/common/national_focus", $"focus_{focuses[0].Id}_tree.txt");
                focuses[0].GenerateFocusTreeFile(focuses, fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: "  + ex.Message);
            }
        }
    }
}