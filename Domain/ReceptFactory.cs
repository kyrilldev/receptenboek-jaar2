using System;
using System.Collections.Generic;

namespace Receptenboek.Domain
{
    public static class ReceptFactory
    {
        public static Recept MaakRecept(
            ReceptType type,
            string naam,
            string omschrijving,
            List<Ingredient> ingredienten,
            List<Bereidingsstap> stappen,
            string? extraKenmerk = null)
        {
            return type switch
            {
                ReceptType.Hoofdgerecht => new HoofdgerechtRecept(
                    naam, omschrijving, ingredienten, stappen, extraKenmerk ?? "Gemiddeld"),
                ReceptType.Vegetarisch => new VegetarischRecept(
                    naam, omschrijving, ingredienten, stappen, extraKenmerk ?? "100% Vegetarisch"),
                ReceptType.Nagerecht => new NagerechtRecept(
                    naam, omschrijving, ingredienten, stappen, extraKenmerk ?? "Warm of Koud"),
                ReceptType.Algemeen => new Recept(
                    naam, omschrijving, ingredienten, stappen),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Onbekend recepttype: {type}")
            };
        }
    }
}
