using System;
using System.Collections.Generic;
using System.Linq;

namespace Receptenboek
{
    public static class Receptenboek
    {
        private static readonly ReceptenManager _manager = new();

        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("==================================================");
            Console.WriteLine("        📖 WELKOM BIJ HET RECEPTENBOEK 📖");
            Console.WriteLine("==================================================");

            bool afsluiten = false;
            while (!afsluiten)
            {
                ToonMenu();
                int keuze = ConsoleHelper.LeesInt("Maak een keuze [1-6]: ", 1, 6);

                switch (keuze)
                {
                    case 1:
                        ToonOverzicht();
                        break;
                    case 2:
                        ZoekRecepten();
                        break;
                    case 3:
                        BekijkReceptDetails();
                        break;
                    case 4:
                        VoegNieuwReceptToe();
                        break;
                    case 5:
                        VerwijderRecept();
                        break;
                    case 6:
                        afsluiten = true;
                        Console.WriteLine("\nBedankt voor het gebruiken van het Receptenboek. Tot ziens!");
                        break;
                }
            }
        }

        private static void ToonMenu()
        {
            Console.WriteLine("\n---------------- MAIN MENU ----------------");
            Console.WriteLine("1. Overzicht van alle recepten tonen");
            Console.WriteLine("2. Recept opzoeken (op naam of ingrediënt)");
            Console.WriteLine("3. Recept details bekijken & schalen");
            Console.WriteLine("4. Nieuw recept toevoegen");
            Console.WriteLine("5. Recept verwijderen");
            Console.WriteLine("6. Afsluiten");
            Console.WriteLine("-------------------------------------------");
        }

        private static void ToonOverzicht()
        {
            var recepten = _manager.GetAlleRecepten();
            Console.WriteLine($"\n📋 BESCHIKBARE RECEPTEN ({recepten.Count}):");
            if (recepten.Count == 0)
            {
                Console.WriteLine("  (Er zijn momenteel geen recepten beschikbaar)");
                return;
            }

            for (int i = 0; i < recepten.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {recepten[i].Naam} ({recepten[i].Categorie})");
            }
        }

        private static void ZoekRecepten()
        {
            string zoekterm = ConsoleHelper.LeesTekst("\n🔍 Voer een zoekterm in (op naam of ingrediënt): ");
            var resultaten = _manager.ZoekRecepten(zoekterm);

            if (resultaten.Count == 0)
            {
                Console.WriteLine($"⚠️ Geen recepten gevonden die voldoen aan zoekterm '{zoekterm}'.");
                return;
            }

            Console.WriteLine($"\n🔎 GEVONDEN RECEPTEN ({resultaten.Count}):");
            for (int i = 0; i < resultaten.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {resultaten[i].Naam} ({resultaten[i].Categorie})");
            }

            if (ConsoleHelper.LeesJaNee("\nWilt u een van deze recepten direct bekijken?"))
            {
                int index = ConsoleHelper.LeesInt($"Selecteer een recept [1-{resultaten.Count}]: ", 1, resultaten.Count) - 1;
                ToonReceptMetOpties(resultaten[index]);
            }
        }

        private static void BekijkReceptDetails()
        {
            var recepten = _manager.GetAlleRecepten();
            if (recepten.Count == 0)
            {
                Console.WriteLine("⚠️ Er zijn geen recepten om te bekijken.");
                return;
            }

            ToonOverzicht();
            int index = ConsoleHelper.LeesInt($"\nKies het nummer van het recept dat u wilt bekijken [1-{recepten.Count}]: ", 1, recepten.Count) - 1;
            ToonReceptMetOpties(recepten[index]);
        }

        private static void ToonReceptMetOpties(Recept recept)
        {
            // REQ-14: Vraag aantal personen en plantaardige keuze
            int aantalPersonen = ConsoleHelper.LeesInt("Voor hoeveel personen wilt u het recept bereiden? ", 1, 100, standaardWaarde: 1);

            bool plantaardig = false;
            if (recept.HeeftPlantaardigeOpties())
            {
                plantaardig = ConsoleHelper.LeesJaNee("Dit recept bevat ingrediënten met plantaardige alternatieven. Wilt u de plantaardige variant gebruiken?");
            }

            recept.ToonDetails(aantalPersonen, plantaardig);
        }

        private static void VoegNieuwReceptToe()
        {
            Console.WriteLine("\n➕ NIEUW RECEPT TOEVOEGEN");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Kies het type recept:");
            Console.WriteLine("  1. Hoofdgerecht");
            Console.WriteLine("  2. Vegetarisch Recept");
            Console.WriteLine("  3. Nagerecht / Ontbijt");
            Console.WriteLine("  4. Algemeen Recept");

            int typeKeuze = ConsoleHelper.LeesInt("Selecteer type [1-4]: ", 1, 4);
            string naam = ConsoleHelper.LeesTekst("Naam recept: ");
            string omschrijving = ConsoleHelper.LeesTekst("Omschrijving recept: ");

            // Ingrediënten toevoegen
            var ingredienten = new List<Ingredient>();
            Console.WriteLine("\n🛒 INGREDIEËNTEN TOEVOEGEN:");
            bool nogEenIngredient = true;
            while (nogEenIngredient)
            {
                string ingrNaam = ConsoleHelper.LeesTekst("  • Naam ingrediënt: ");
                double hoeveelheid = ConsoleHelper.LeesDouble("  • Hoeveelheid (per 1 persoon): ", min: 0.01);
                string eenheid = ConsoleHelper.LeesTekst("  • Eenheid (bijv. g, ml, stuks, el): ");
                int kcal = ConsoleHelper.LeesInt("  • Energiewaarde kcal (per 1 persoon): ", min: 0);

                string? plantAlt = null;
                int? plantKcal = null;
                if (ConsoleHelper.LeesJaNee("  • Heeft dit ingrediënt een plantaardig alternatief?"))
                {
                    plantAlt = ConsoleHelper.LeesTekst("    - Naam plantaardig alternatief: ");
                    plantKcal = ConsoleHelper.LeesInt("    - Energiewaarde kcal voor alternatief (per 1 persoon): ", min: 0);
                }

                ingredienten.Add(new Ingredient(ingrNaam, hoeveelheid, eenheid, kcal, plantAlt, plantKcal));
                nogEenIngredient = ConsoleHelper.LeesJaNee("Nog een ingrediënt toevoegen?");
            }

            // Bereidingsstappen toevoegen
            var stappen = new List<Bereidingsstap>();
            Console.WriteLine("\n👨‍🍳 BEREIDINGSSTAPPEN TOEVOEGEN:");
            bool nogEenStap = true;
            int stapTeller = 1;
            while (nogEenStap)
            {
                string beschrijving = ConsoleHelper.LeesTekst($"  Stap {stapTeller} beschrijving: ");
                int duur = ConsoleHelper.LeesInt($"  Stap {stapTeller} duur in minuten: ", min: 0);

                string? tip = null;
                if (ConsoleHelper.LeesJaNee($"  Heeft stap {stapTeller} een tip?"))
                {
                    tip = ConsoleHelper.LeesTekst("    💡 Voer de tip in: ");
                }

                stappen.Add(new Bereidingsstap(beschrijving, duur, tip));
                stapTeller++;
                nogEenStap = ConsoleHelper.LeesJaNee("Nog een bereidingsstap toevoegen?");
            }

            Recept nieuwRecept;
            switch (typeKeuze)
            {
                case 1:
                    string moeilijkheid = ConsoleHelper.LeesTekst("Moeilijkheidsgraad (bijv. Makkelijk, Gemiddeld, Chef): ");
                    nieuwRecept = new HoofdgerechtRecept(naam, omschrijving, ingredienten, stappen, moeilijkheid);
                    break;
                case 2:
                    string keurmerk = ConsoleHelper.LeesTekst("Dieetkeurmerk (bijv. 100% Vegetarisch, Vegan): ");
                    nieuwRecept = new VegetarischRecept(naam, omschrijving, ingredienten, stappen, keurmerk);
                    break;
                case 3:
                    string temp = ConsoleHelper.LeesTekst("Serveertemperatuur (bijv. Warm, Koud, IJskoud): ");
                    nieuwRecept = new NagerechtRecept(naam, omschrijving, ingredienten, stappen, temp);
                    break;
                default:
                    nieuwRecept = new Recept(naam, omschrijving, ingredienten, stappen);
                    break;
            }

            _manager.VoegReceptToe(nieuwRecept);
            Console.WriteLine($"\n✅ Recept '{naam}' is succesvol toegevoegd aan het receptenboek!");
        }

        private static void VerwijderRecept()
        {
            var recepten = _manager.GetAlleRecepten();
            if (recepten.Count == 0)
            {
                Console.WriteLine("⚠️ Er zijn geen recepten om te verwijderen.");
                return;
            }

            Console.WriteLine("\n🗑️ RECEPT VERWIJDEREN");
            ToonOverzicht();

            int index = ConsoleHelper.LeesInt($"\nSelecteer het recept dat u wilt verwijderen [1-{recepten.Count}]: ", 1, recepten.Count) - 1;
            var teVerwijderen = recepten[index];

            if (ConsoleHelper.LeesJaNee($"Weet u zeker dat u '{teVerwijderen.Naam}' wilt verwijderen?"))
            {
                _manager.VerwijderRecept(teVerwijderen);
                Console.WriteLine($"✅ Recept '{teVerwijderen.Naam}' is definitief verwijderd.");
            }
            else
            {
                Console.WriteLine("Verwijderen geannuleerd.");
            }
        }
    }
}