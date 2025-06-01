namespace H4NationalFocusGUI.functional
{
    public static class FolderStructure
    {
        public static void CreateStructure()
        {
            if (!Directory.Exists("mod"))
            {
                Directory.CreateDirectory("mod");
                Directory.CreateDirectory("mod/common");
                Directory.CreateDirectory("mod/common/national_focus");

                Directory.CreateDirectory("mod/gfx");
                Directory.CreateDirectory("mod/gfx/interface");
                Directory.CreateDirectory("mod/gfx/interface/goals");

                Directory.CreateDirectory("mod/interface");

                Directory.CreateDirectory("mod/localisation");
            }
            else
            {
                Console.WriteLine("mod folder not Exist!");
                Console.WriteLine("Creating mod Structure");
            }
        }
    }
}