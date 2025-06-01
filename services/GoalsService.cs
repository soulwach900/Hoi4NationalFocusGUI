using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class GoalsService
    {
        public void CreateGoalsGfx(List<Focus> focuses, string countryName)
        {
            var path = $"mod/interface/{countryName}_goals.gfx";
            using StreamWriter writer = new StreamWriter(path);
            writer.WriteLine("spriteTypes = {");

            foreach (var focus in focuses)
            {
                var id = focus.Id.Replace(" ", "_").ToLower();
                var iconFile = focus.Icon.Replace("GFX_goal_", "") + ".dds";

                writer.WriteLine("\tspriteType = {");
                writer.WriteLine($"\t\tname = \"GFX_goal_{id}\"");
                writer.WriteLine($"\t\ttexturefile = \"gfx/interface/goals/{iconFile}\"");
                writer.WriteLine("\t}");
            }

            writer.WriteLine("}");
        }
    }
}