using System.Collections.Generic;

namespace Receptenboek
{
    public class VegetarischRecept : Recept
    {
        public string DieetKeurmerk { get; set; }

        public override string Categorie => "Vegetarisch Recept";

        public VegetarischRecept(string naam, string omschrijving, List<Ingredient>? ingredienten = null, List<Bereidingsstap>? bereidingsstappen = null, string dieetKeurmerk = "100% Vegetarisch")
            : base(naam, omschrijving, ingredienten, bereidingsstappen)
        {
            DieetKeurmerk = dieetKeurmerk;
        }

        protected override void ToonExtraDetails()
        {
            System.Console.WriteLine($"   Keurmerk: 🌱 {DieetKeurmerk}");
        }
    }
}
