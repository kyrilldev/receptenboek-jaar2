using System.Collections.Generic;

namespace Receptenboek
{
    public class HoofdgerechtRecept : Recept
    {
        public string Moeilijkheidsgraad { get; set; }

        public override string Categorie => "Hoofdgerecht";

        public HoofdgerechtRecept(string naam, string omschrijving, List<Ingredient>? ingredienten = null, List<Bereidingsstap>? bereidingsstappen = null, string moeilijkheidsgraad = "Gemiddeld")
            : base(naam, omschrijving, ingredienten, bereidingsstappen)
        {
            Moeilijkheidsgraad = moeilijkheidsgraad;
        }

        protected override void ToonExtraDetails()
        {
            System.Console.WriteLine($"   Moeilijkheidsgraad: 👨‍🍳 {Moeilijkheidsgraad}");
        }
    }
}
