namespace H4NationalFocusGUI.components
{
    public class Focus
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; } = 10;
        public List<string> Prerequisites { get; set; } = new List<string>();

        public Focus(string id, string icon, string name, string description, int x, int y, int cost)
        {
            Id = id;
            Icon = icon;
            Name = name;
            Description = description;
            X = x;
            Y = y;
            Cost = cost;
        }

        public void GenerateFocusTreeFile(List<Focus> focuses, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("focus_tree = {");
                writer.WriteLine("\tid = my_country_focus");

                foreach (var focus in focuses)
                {
                    writer.WriteLine("\tfocus = {");
                    writer.WriteLine($"\t\tid = {focus.Id}");
                    writer.WriteLine($"\t\ticon = {focus.Icon}");
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
                            writer.WriteLine($"\t\t\t{{ focus = {prereq.Trim()} }}");
                        writer.WriteLine("\t\t}");
                    }

                    writer.WriteLine("\t}");
                }

                writer.WriteLine("}");
            }
        }
    }
}