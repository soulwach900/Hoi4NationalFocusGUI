using System.Text;
using H4NationalFocusGUI.components;

namespace H4NationalFocusGUI.services
{
    public class LocalisationService
    {
        public void CreateLocalisation(List<Focus> focuses, string countryName)
        {
            var safeCountryName = countryName.Replace(" ", "_").ToLowerInvariant();
            using var writer = new StreamWriter($"mod/localisation/{safeCountryName}_l_english.yml", false, new UTF8Encoding(true));
            
            writer.WriteLine("l_english:");

            foreach (var focus in focuses)
            {
                var id = focus.Id.Replace(" ", "_").ToLower();
                var name = focus.Name;
                var desc = focus.Description;

                writer.WriteLine($"\t{id}:0 \"{name}\"");
                writer.WriteLine($"\t{id}_desc:0 \"{desc}\"");
            }
        }
    }
}