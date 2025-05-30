using H4NationalFocusGUI.components;
using System.Text;

namespace H4NationalFocusGUI.functional
{
    public class LocalisationService
    {
        public void CreateLocalisation(List<Focus> focuses, string countryName)
        {
            string safeCountryName = countryName.Replace(" ", "_").ToLowerInvariant();
            using (StreamWriter writer = new StreamWriter($"mod/localisation/{safeCountryName}_l_english.yml", false, new UTF8Encoding(true)))
            {
                writer.WriteLine("l_english:");

                foreach (var focus in focuses)
                {
                    string id = focus.Id.Replace(" ", "_").ToLower();
                    string name = focus.Name;
                    string desc = focus.Description;

                    writer.WriteLine($"\t{id}:0 \"{name}\"");
                    writer.WriteLine($"\t{id}_desc:0 \"{desc}\"");
                }
            }
        }
    }
}