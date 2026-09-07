using System;
using System.Collections.Generic;
using System.Linq;

namespace Receptenboek.Domain
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

        public virtual string? ExtraInformatie => null;

        public virtual float BerekenTotaleBereidingsTijd()
        {
            return Bereidingsstappen.Sum(s => s.DuurMinuten);
        }
    }
}
