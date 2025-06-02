using Gtk;

namespace H4NationalFocusGUI.services
{
    
    public static class ExplorerService
    {
        public static string? FileSelector(string title, string filterName, string[] extensions)
        {
            string? path = null;

            using var dialog = new FileChooserDialog(
                title,
                null,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept
            );
                
            var filter = new FileFilter { Name = filterName };
            foreach (var ext in extensions)
            {
                filter.AddPattern("*." + ext.TrimStart('.'));
            }
            dialog.AddFilter(filter);

            if (dialog.Run() == (int)ResponseType.Accept)
            {
                path = dialog.Filename;
                Console.WriteLine("Selected File: " + path);
            }
            else
            {
                Console.WriteLine("No File Selected.");
            }

            return path;
        }
    }
}