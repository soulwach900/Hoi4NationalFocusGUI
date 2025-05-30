using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class FocusSaveService
    {
        public (bool Success, string Message) TryAddFocus(
            string id, string name, string desc,
            string x, string y, string cost, string icon,
            List<string> selectedPrerequisites,
            List<Focus> focuses)
        {
            if (string.IsNullOrWhiteSpace(id))
                return (false, "Focus ID is required.");

            id = id.Replace(" ", "_").ToLower();

            if (!int.TryParse(x, out int xVal) ||
                !int.TryParse(y, out int yVal) ||
                !int.TryParse(cost, out int costVal))
            {
                return (false, "Invalid numeric values for X, Y, or cost.");
            }

            var focus = new Focus(id, icon, name, desc, xVal, yVal, costVal);
            focus.Prerequisites.AddRange(selectedPrerequisites);
            focuses.Add(focus);

            return (true, "Focus saved successfully.");
        }

        public (bool Success, string Message) SaveFocusYaml(List<Focus> focuses)
        {
            if (focuses.Count == 0)
                return (false, "No focus created to save.");

            try
            {
                string fullPath = Path.Combine("mod/common/national_focus", $"focus_{focuses[0].Id}_tree.txt");
                focuses[0].GenerateFocusTreeFile(focuses, fullPath);
                return (true, "File saved to desktop.");
            }
            catch (Exception ex)
            {
                return (false, $"Error saving file: {ex.Message}");
            }
        }
    }
}