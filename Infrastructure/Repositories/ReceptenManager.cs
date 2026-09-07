using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Receptenboek.Domain;
using Receptenboek.Application.Interfaces;

namespace Receptenboek.Infrastructure.Repositories
{
    public class ReceptenManager : IReceptenRepository
    {
        private readonly List<Recept> _recepten = new();
        private readonly string? _filePath;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public ReceptenManager(string? filePath = "recepten.json")
        {
            _filePath = filePath;
            LaadRecepten();
        }

        private void LaadRecepten()
        {
            // OPT-REQ-01: Laden uit extern gegevensbestand bij start
            if (string.IsNullOrWhiteSpace(_filePath) || !File.Exists(_filePath))
            {
                LaadBeginRecepten();
                SlaReceptenOp();
                return;
            }

            // OPT-REQ-03: Melding bij ongeldig bestand, geen crash (graceful fallback)
            try
            {
                string json = File.ReadAllText(_filePath);
                var geladenRecepten = JsonSerializer.Deserialize<List<Recept>>(json, _jsonOptions);
                if (geladenRecepten != null && geladenRecepten.Count > 0)
                {
                    _recepten.Clear();
                    _recepten.AddRange(geladenRecepten);
                    return;
                }
            }
            catch (Exception ex) when (ex is JsonException or IOException)
            {
                Console.WriteLine($"⚠️ [OPT-REQ-03] Waarschuwing: Kan '{_filePath}' niet inladen ({ex.Message}). Standaardrecepten worden geladen.");
            }

            LaadBeginRecepten();
        }

        // OPT-REQ-02: Wijzigingen persistent opslaan
        public void SlaReceptenOp()
        {
            if (string.IsNullOrWhiteSpace(_filePath)) return;

            try
            {
                string json = JsonSerializer.Serialize(_recepten, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Fout bij opslaan van recepten naar '{_filePath}': {ex.Message}");
            }
        }

        private void LaadBeginRecepten()
        {
            _recepten.Clear();

            // Recept 1: Spaghetti Bolognese (Hoofdgerecht)
            var r1 = new HoofdgerechtRecept(
                "Spaghetti Bolognese",
                "Klassieke Italiaanse pastasaus met rundergehakt (of vegetarisch gehakt), tomaten en verse kruiden.",
                new List<Ingredient>
                {
                    new Ingredient("Spaghetti", 100, "g", 350),
                    new Ingredient("Rundergehakt", 125, "g", 300, "Vegetarisch gehakt", 220),
                    new Ingredient("Gezeefde tomaten (passata)", 150, "ml", 45),
                    new Ingredient("Ui", 0.5, "stuks", 20),
                    new Ingredient("Knoflook", 1, "tenetje", 5),
                    new Ingredient("Olijfolie", 10, "ml", 80)
                },
                new List<Bereidingsstap>
                {
                    new Bereidingsstap("Snipper de ui en hak de knoflook fijn.", 5, "Fruit de ui op laag vuur tot hij glazig is voor de beste smaak."),
                    new Bereidingsstap("Ril het gehakt (of vega gehakt) bruin in de olijfolie in een ruime pan.", 7),
                    new Bereidingsstap("Voeg de gezeefde tomaten, ui en knoflook toe en laat zachtjes pruttelen.", 15, "Voeg een snufje suiker toe om de zuren van de tomaat te balanceren."),
                    new Bereidingsstap("Kook de spaghetti in gezouten water volgens de verpakking al dente.", 9),
                    new Bereidingsstap("Serveerde saus over de verse spaghetti.", 2)
                },
                "Gemiddeld"
            );

            // Recept 2: Groene Groenten Curry (Vegetarisch)
            var r2 = new VegetarischRecept(
                "Groene Groenten Curry",
                "Aromatische Thaise groene curry met verse groenten en romige kokosmelk.",
                new List<Ingredient>
                {
                    new Ingredient("Basmatirijst", 85, "g", 300),
                    new Ingredient("Groene currypasta", 20, "g", 30),
                    new Ingredient("Kokosmelk", 150, "ml", 280),
                    new Ingredient("Broccoli", 100, "g", 35),
                    new Ingredient("Tofu", 120, "g", 100, "Vega kipstuckjes", 150),
                    new Ingredient("Peultjes", 75, "g", 25)
                },
                new List<Bereidingsstap>
                {
                    new Bereidingsstap("Snijd de tofu in blokjes en bak deze goudbruin in een koekenpan.", 8, "Dep de tofu droog met keukenpapier voor een knapperige korst."),
                    new Bereidingsstap("Kook de basmatirijst volgens de aanwijzingen op de verpakking.", 12),
                    new Bereidingsstap("Fruit de groene currypasta kort in een wokpan en giet de kokosmelk erbij.", 3),
                    new Bereidingsstap("Voeg de broccoli, peultjes en gebakken tofu toe en laat zachtjes koken.", 8, "Voeg op het laatst wat verse koriander of limoensap toe."),
                    new Bereidingsstap("Server de warme curry met de gestoomde rijst.", 2)
                },
                "100% Vegetarisch & Veganistisch"
            );

            // Recept 3: Luchtige Pannenkoeken (Nagerecht / Ontbijt)
            var r3 = new NagerechtRecept(
                "Luchtige Pannenkoeken met Appel",
                "Heerlijk traditionele Nederlandse pannenkoeken met gebakken appelschijfjes en kaneel.",
                new List<Ingredient>
                {
                    new Ingredient("Tarwebloem", 100, "g", 340),
                    new Ingredient("Koemelk", 200, "ml", 100, "Havermelk", 90),
                    new Ingredient("Ei", 1, "stuks", 70, "Appelmoes (50g)", 40),
                    new Ingredient("Appel", 1, "stuks", 60),
                    new Ingredient("Boter", 15, "g", 110, "Plantaardige margarine", 100),
                    new Ingredient("Kaneel", 1, "g", 2)
                },
                new List<Bereidingsstap>
                {
                    new Bereidingsstap("Meng de bloem, melk en het ei tot een glad beslag.", 5, "Laat het beslag 10 minuten rusten voor extra luchtige pannenkoeken."),
                    new Bereidingsstap("Schil de appel en snijd in dunne schijfjes.", 4),
                    new Bereidingsstap("Smelt boter in de pan, verdeel appelschijfjes en giet een pollepel beslag erover.", 3),
                    new Bereidingsstap("Bak de pannenkoek aan beide kanten goudbruin.", 4, "Draai de pannenkoek pas om als de bovenkant droog is.")
                },
                "Warm geserveerd"
            );

            _recepten.Add(r1);
            _recepten.Add(r2);
            _recepten.Add(r3);
        }

        public List<Recept> GetAlleRecepten() => _recepten.ToList();

        public List<Recept> ZoekRecepten(string zoekterm)
        {
            return _recepten.Where(r => r.VoldoetAanZoekterm(zoekterm)).ToList();
        }

        public Recept? ZoekOpExacteNaam(string naam)
        {
            return _recepten.FirstOrDefault(r => r.Naam.Equals(naam.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public void VoegReceptToe(Recept recept)
        {
            _recepten.Add(recept);
            SlaReceptenOp();
        }

        public bool VerwijderRecept(Recept recept)
        {
            bool isVerwijderd = _recepten.Remove(recept);
            if (isVerwijderd)
            {
                SlaReceptenOp();
            }
            return isVerwijderd;
        }
    }
}
