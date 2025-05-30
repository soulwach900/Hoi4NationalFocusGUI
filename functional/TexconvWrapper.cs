using System.Diagnostics;

namespace H4NationalFocusGUI.functional
{
    public class TexconvWrapper
    {
        public static bool ConvertPngToDds(string texconvPath, string inputPngPath, string outputDirectory, string focusId)
        {
            if (!File.Exists(texconvPath) || !File.Exists(inputPngPath))
                return false;

            string tempInputCopy = Path.Combine(Path.GetTempPath(), $"{focusId}.png");
            File.Copy(inputPngPath, tempInputCopy, true);

            var process = new Process();
            process.StartInfo.FileName = texconvPath;
            process.StartInfo.Arguments = $"-f DXT5 -nologo -y -o \"{outputDirectory}\" \"{tempInputCopy}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine("texconv error:");
                Console.WriteLine(stderr);
                return false;
            }

            return true;
        }
    }
}