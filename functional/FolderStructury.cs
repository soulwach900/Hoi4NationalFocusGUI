namespace H4NationalFocusGUI.functional
{
    public class FolderStructury
    {
        public void CreateStructury()
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
                Console.WriteLine("mod folder not Existe!");
                Console.WriteLine("Creating mod Structury");
            }
        }
    }
}