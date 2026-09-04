using System.Collections.Generic;

namespace Receptenboek.Domain
{
    public class NagerechtRecept : Recept
    {
        public string Serveertemperatuur { get; set; }

        public override string Categorie => "Nagerecht / Ontbijt";

        public NagerechtRecept(string naam, string omschrijving, List<Ingredient>? ingredienten = null, List<Bereidingsstap>? bereidingsstappen = null, string serveertemperatuur = "Warm of Koud")
            : base(naam, omschrijving, ingredienten, bereidingsstappen)
        {
            Serveertemperatuur = serveertemperatuur;
        }

        protected override void ToonExtraDetails()
        {
            System.Console.WriteLine($"   Serveren: 🍧 {Serveertemperatuur}");
        }
    }
}
