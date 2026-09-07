using System.Collections.Generic;

namespace Receptenboek.Domain
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

        public override string? ExtraInformatie => $"Moeilijkheidsgraad: 👨‍🍳 {Moeilijkheidsgraad}";
    }
}
