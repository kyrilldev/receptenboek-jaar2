using System;
using Receptenboek.Domain;

namespace Receptenboek.Presentation
{
    public static class ReceptPresenter
    {
        public static void ToonDetails(Recept recept, int aantalPersonen = 1, bool gebruikPlantaardig = false)
        {
            ToonHeader(recept, aantalPersonen, gebruikPlantaardig);
            ToonIngredienten(recept, aantalPersonen, gebruikPlantaardig);
            ToonBereidingsstappen(recept);
            Console.WriteLine("==================================================\n");
        }

        private static void ToonHeader(Recept recept, int aantalPersonen, bool gebruikPlantaardig)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine($"📖 RECEPT: {recept.Naam.ToUpper()}");
            Console.WriteLine($"   Categorie: {recept.Categorie}");
            if (!string.IsNullOrWhiteSpace(recept.ExtraInformatie))
            {
                Console.WriteLine($"   {recept.ExtraInformatie}");
            }
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Omschrijving: {recept.Omschrijving}");
            string variantLabel = (gebruikPlantaardig && recept.HeeftPlantaardigeOpties()) ? " (Plantaardige variant)" : "";
            Console.WriteLine($"Aantal personen: {aantalPersonen}{variantLabel}");
            Console.WriteLine("--------------------------------------------------");
        }

        private static void ToonIngredienten(Recept recept, int aantalPersonen, bool gebruikPlantaardig)
        {
            Console.WriteLine("\n🛒 INGREDIEËNTEN (opsomming):");
            if (recept.Ingredienten.Count == 0)
            {
                Console.WriteLine("  (Geen ingrediënten vermeld)");
                return;
            }

            foreach (var ingr in recept.Ingredienten)
            {
                double hoeveelheid = ingr.GetHoeveelheid(aantalPersonen);
                string naam = ingr.GetNaam(gebruikPlantaardig);
                int kcal = ingr.GetKcal(aantalPersonen, gebruikPlantaardig);
                string notaPlantaardig = (gebruikPlantaardig && ingr.HeeftPlantaardigAlternatief) ? " 🌱 [Plantaardig]" : "";
                Console.WriteLine($"  • {hoeveelheid:0.##} {ingr.Eenheid} {naam}{notaPlantaardig} ({kcal} kcal)");
            }

            int totaalKcal = recept.BerekenTotaalKcal(aantalPersonen, gebruikPlantaardig);
            string persWoord = aantalPersonen == 1 ? "persoon" : "personen";
            Console.WriteLine($"\n🔥 Totale energiewaarde: {totaalKcal} kcal (voor {aantalPersonen} {persWoord})");
        }

        private static void ToonBereidingsstappen(Recept recept)
        {
            Console.WriteLine("\n👨‍🍳 BEREIDINGSSTAPPEN (genummerd):");
            if (recept.Bereidingsstappen.Count == 0)
            {
                Console.WriteLine("  (Geen bereidingsstappen vermeld)");
                return;
            }

            for (int i = 0; i < recept.Bereidingsstappen.Count; i++)
            {
                var stap = recept.Bereidingsstappen[i];
                Console.WriteLine($"  {i + 1}. {stap.Beschrijving} [{stap.DuurMinuten} min]");
                if (stap.HeeftTip)
                {
                    Console.WriteLine($"     💡 Tip: {stap.Tip}");
                }
            }
        }
    }
}
