namespace Receptenboek
{
    public class Ingredient
    {
        public string Naam { get; set; }
        public double Hoeveelheid { get; set; } // Basisportie per 1 persoon
        public string Eenheid { get; set; } // bijv. "g", "ml", "stuks", "el"
        public int Kcal { get; set; } // Kcal per 1 persoon
        
        public string? PlantaardigAlternatief { get; set; }
        public int? PlantaardigeKcal { get; set; }

        public bool HeeftPlantaardigAlternatief => !string.IsNullOrWhiteSpace(PlantaardigAlternatief);

        public Ingredient(string naam, double hoeveelheid, string eenheid = "g", int kcal = 0, string? plantaardigAlternatief = null, int? plantaardigeKcal = null)
        {
            Naam = naam;
            Hoeveelheid = hoeveelheid;
            Eenheid = eenheid;
            Kcal = kcal;
            PlantaardigAlternatief = plantaardigAlternatief;
            PlantaardigeKcal = plantaardigeKcal;
        }

        public string GetNaam(bool gebruikPlantaardig = false)
        {
            return (gebruikPlantaardig && HeeftPlantaardigAlternatief) ? PlantaardigAlternatief! : Naam;
        }

        public double GetHoeveelheid(int aantalPersonen)
        {
            return Hoeveelheid * aantalPersonen;
        }

        public int GetKcal(int aantalPersonen, bool gebruikPlantaardig)
        {
            int basisKcal = (gebruikPlantaardig && HeeftPlantaardigAlternatief && PlantaardigeKcal.HasValue)
                ? PlantaardigeKcal.Value
                : Kcal;

            return basisKcal * aantalPersonen;
        }
    }
}
