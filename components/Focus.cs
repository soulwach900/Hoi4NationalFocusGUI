namespace H4NationalFocusGUI.components
{
    public class Focus(string id, string iconId, string name, string description, int x, int y, int cost)
    {
        public string Id { get; set; } = id;
        public string IconId { get; set; } = iconId;
        public string IconPath { get; set; } = "";
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Cost { get; set; } = cost;
        public List<string> Prerequisites { get; set; } = [];

        public void GenerateFocusTreeFile(List<Focus> focuses, string fileName)
        {
            using StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine("focus_tree = {");
            writer.WriteLine("\tid = my_country_focus");

            foreach (var focus in focuses)
            {
                writer.WriteLine("\tfocus = {");
                writer.WriteLine($"\t\tid = {focus.Id}");
                writer.WriteLine($"\t\ticon = {focus.IconId}");
                writer.WriteLine($"\t\tx = {focus.X}");
                writer.WriteLine($"\t\ty = {focus.Y}");
                writer.WriteLine($"\t\tcost = {focus.Cost}");
                writer.WriteLine($"\t\ttargeted = no");
                writer.WriteLine($"\t\tcompletion_reward = {{ }}");
                writer.WriteLine($"\t\tai_will_do = {{ factor = 1 }}");

                if (focus.Prerequisites.Count > 0)
                {
                    writer.WriteLine("\t\tprerequisite = {");
                    foreach (var prereq in focus.Prerequisites)
                    {
                        writer.WriteLine($"\t\t\tfocus = {prereq.Trim()}");
                    }
                    writer.WriteLine("\t\t}");
                }

                writer.WriteLine("\t}");
            }

            writer.WriteLine("}");
        }
    }
}