using System;
using System.Collections.Generic;
using System.Linq;

namespace Receptenboek
{
    public class Recept
    {
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public List<Ingredient> Ingredienten { get; set; }
        public List<Bereidingsstap> Bereidingsstappen { get; set; }

        public virtual string Categorie => "Algemeen Recept";

        public Recept(string naam, string omschrijving, List<Ingredient>? ingredienten = null, List<Bereidingsstap>? bereidingsstappen = null)
        {
            Naam = naam;
            Omschrijving = omschrijving;
            Ingredienten = ingredienten ?? new List<Ingredient>();
            Bereidingsstappen = bereidingsstappen ?? new List<Bereidingsstap>();
        }

        public bool HeeftPlantaardigeOpties()
        {
            return Ingredienten.Any(i => i.HeeftPlantaardigAlternatief);
        }

        public int BerekenTotaalKcal(int aantalPersonen, bool gebruikPlantaardig)
        {
            return Ingredienten.Sum(i => i.GetKcal(aantalPersonen, gebruikPlantaardig));
        }

        public virtual bool VoldoetAanZoekterm(string zoekterm)
        {
            if (string.IsNullOrWhiteSpace(zoekterm)) return true;

            string term = zoekterm.Trim().ToLowerInvariant();

            if (Naam.ToLowerInvariant().Contains(term) || Omschrijving.ToLowerInvariant().Contains(term))
                return true;

            if (Ingredienten.Any(i => i.Naam.ToLowerInvariant().Contains(term) || 
                                     (i.HeeftPlantaardigAlternatief && i.PlantaardigAlternatief!.ToLowerInvariant().Contains(term))))
                return true;

            return false;
        }

        public virtual void ToonDetails(int aantalPersonen = 1, bool gebruikPlantaardig = false)
        {
            Console.WriteLine($"\n==================================================");
            Console.WriteLine($"📖 RECEPT: {Naam.ToUpper()}");
            Console.WriteLine($"   Categorie: {Categorie}");
            ToonExtraDetails();
            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"Omschrijving: {Omschrijving}");
            Console.WriteLine($"Aantal personen: {aantalPersonen}" + (gebruikPlantaardig && HeeftPlantaardigeOpties() ? " (Plantaardige variant)" : ""));
            Console.WriteLine($"--------------------------------------------------");

            Console.WriteLine("\n🛒 INGREDIEËNTEN (opsomming):");
            if (Ingredienten.Count == 0)
            {
                Console.WriteLine("  (Geen ingrediënten vermeld)");
            }
            else
            {
                foreach (var ingr in Ingredienten)
                {
                    double hoeveelheid = ingr.GetHoeveelheid(aantalPersonen);
                    string naam = ingr.GetNaam(gebruikPlantaardig);
                    int kcal = ingr.GetKcal(aantalPersonen, gebruikPlantaardig);

                    string notaPlantaardig = (gebruikPlantaardig && ingr.HeeftPlantaardigAlternatief) ? " 🌱 [Plantaardig]" : "";
                    Console.WriteLine($"  • {hoeveelheid:0.##} {ingr.Eenheid} {naam}{notaPlantaardig} ({kcal} kcal)");
                }
            }

            int totaalKcal = BerekenTotaalKcal(aantalPersonen, gebruikPlantaardig);
            Console.WriteLine($"\n🔥 Totale energiewaarde: {totaalKcal} kcal (voor {aantalPersonen} {(aantalPersonen == 1 ? "persoon" : "personen")})");

            Console.WriteLine("\n👨‍🍳 BEREIDINGSSTAPPEN (genummerd):");
            if (Bereidingsstappen.Count == 0)
            {
                Console.WriteLine("  (Geen bereidingsstappen vermeld)");
            }
            else
            {
                for (int i = 0; i < Bereidingsstappen.Count; i++)
                {
                    var stap = Bereidingsstappen[i];
                    Console.WriteLine($"  {i + 1}. {stap.Beschrijving} [{stap.DuurMinuten} min]");
                    if (stap.HeeftTip)
                    {
                        Console.WriteLine($"     💡 Tip: {stap.Tip}");
                    }
                }
            }
            Console.WriteLine($"==================================================\n");
        }

        protected virtual void ToonExtraDetails()
        {
            // Hook voor afgeleide klassen
        }

        public virtual float BerekenTotaleBereidingsTijd()
        {
            return Bereidingsstappen.Sum(s => s.DuurMinuten);
        }
    }
}
