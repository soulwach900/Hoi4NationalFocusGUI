using ImageMagick;

namespace H4NationalFocusGUI.functional
{
    public static class MagickConverter
    {
        public static void ConvertPngToDds(string inputPngPath, string outputDirectory, string focusId)
        {
            if (!File.Exists(inputPngPath))
            {
                Console.WriteLine($"PNG Not Found: {inputPngPath}");
                return;
            }

            Directory.CreateDirectory(outputDirectory);
            var outputPath = Path.Combine(outputDirectory, $"{focusId}.dds");

            try
            {
                using var image = new MagickImage(inputPngPath);
                
                image.Format = MagickFormat.Dds;
                image.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5");

                image.Write(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Convert PNG: {ex.Message}");
            }
        }
    }
}