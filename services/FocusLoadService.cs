using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class FocusLoadService
    {
        public List<Focus>? ExplorerOpen(ref string statusMessage, ref float statusTimer)
        {
            string? selectedPath = ExplorerService.FileSelector(
                "Choose National Focus",
                "National Focus Files (*.txt)",
                ["txt"]
            );
            
            if (string.IsNullOrEmpty(selectedPath)) return null;
            
            try
            {
                var focuses = LoadFocusTreeFile(selectedPath);

                statusMessage = "National Focus loaded successfully!";
                statusTimer = 3.0f;

                return focuses;
            }
            catch (Exception ex)
            {
                statusMessage = "Error Load National Focus: " + ex.Message;
                statusTimer = 3.0f;
                return null;
            }
        }

        private static List<Focus> LoadFocusTreeFile(string fileName)
        {
            var focuses = new List<Focus>();
            Focus? currentFocus = null;
            bool inFocusBlock = false;
            bool inPrereqBlock = false;

            foreach (var line in File.ReadLines(fileName))
            {
                var trimmed = line.Trim();

                if (trimmed.Contains("focus = {"))
                {
                    currentFocus = new Focus(id: "", iconId: "", name: "", description: "", x: 0, y: 0, cost: 10);
                    inFocusBlock = true;
                    inPrereqBlock = false;
                    continue;
                }

                if (!inFocusBlock || currentFocus == null) continue;
                if (trimmed == "}")
                {
                    focuses.Add(currentFocus);
                    currentFocus = null;
                    inFocusBlock = false;
                    continue;
                }

                if (trimmed.StartsWith("id = "))
                    currentFocus.Id = trimmed.Substring("id = ".Length).Trim();
                else if (trimmed.StartsWith("icon = "))
                    currentFocus.IconId = trimmed.Substring("icon = ".Length).Trim();
                else if (trimmed.StartsWith("x = ") && int.TryParse(trimmed.Substring("x = ".Length), out var x))
                    currentFocus.X = x;
                else if (trimmed.StartsWith("y = ") && int.TryParse(trimmed.Substring("y = ".Length), out var y))
                    currentFocus.Y = y;
                else if (trimmed.StartsWith("cost = ") && int.TryParse(trimmed.Substring("cost = ".Length), out var cost))
                    currentFocus.Cost = cost;

                else if (trimmed.StartsWith("prerequisite ="))
                {
                    if (trimmed.Contains("focus =") && trimmed.EndsWith("}"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(trimmed, @"focus\s*=\s*([^\s}]+)");
                        if (match.Success)
                        {
                            currentFocus.Prerequisites.Add(match.Groups[1].Value.Trim());
                        }
                    }
                    else
                    {
                        inPrereqBlock = true;
                    }
                }

                else if (inPrereqBlock)
                {
                    if (trimmed == "}")
                    {
                        inPrereqBlock = false;
                    }
                    else if (trimmed.StartsWith("focus = "))
                    {
                        var prereqId = trimmed.Substring("focus = ".Length).Trim();
                        currentFocus.Prerequisites.Add(prereqId);
                    }
                }
            }

            return focuses;
        }
    }
}