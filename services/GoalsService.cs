using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public static class GoalsService
    {
        public static void CreateGoalsGfx(List<Focus> focuses, string id)
        {
            var path = $"mod/interface/{id}_goals.gfx";
            using var writer = new StreamWriter(path);
            writer.WriteLine("spriteTypes = {");

            foreach (var focus in focuses)
            {
                var focusId = focus.Id.Replace(" ", "_").ToLower();
                var iconFile = focusId + ".dds";

                writer.WriteLine("\tspriteType = {");
                writer.WriteLine($"\t\tname = \"GFX_goal_{focusId}\"");
                writer.WriteLine($"\t\ttexturefile = \"gfx/interface/goals/{iconFile}\"");
                writer.WriteLine("\t}");
            }

            writer.WriteLine("}");
        }
    }
}