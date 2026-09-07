using System.Collections.Generic;
using System.IO;
using Receptenboek.Domain;
using Receptenboek.Application.Services;
using Receptenboek.Infrastructure.Repositories;

namespace Receptenboek.Tests;

public class UnitTest1
{
    // Schalen van ingrediënten op basis van aantal personen
    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 40)]
    [InlineData(4, 80)]
    [InlineData(45, 900)]
    public void GetHoeveelheid_SchaaltMetAantalPersonen(int aantalPersonen, int verwachteHoeveelheid)
    {
        // Arrange
        var ingredient = new Ingredient("kaas", 20, "g", 80);

        // Act
        var hoeveelheid = ingredient.GetHoeveelheid(aantalPersonen);

        // Assert
        Assert.Equal(verwachteHoeveelheid, hoeveelheid);
    }

    // Plantaardige alternatieven voor ingrediënten
    [Theory]
    [InlineData(false, "melk")]
    [InlineData(true, "havermelk")]
    public void GetNaam_PlantaardigAlternatief_GeeftGekozenVariant(bool gebruikPlantaardig, string verwachteNaam)
    {
        // Arrange
        var ingredient = new Ingredient("melk", 250, "ml", kcal: 110, "havermelk", plantaardigeKcal: 90);

        // Act
        var naam = ingredient.GetNaam(gebruikPlantaardig);

        // Assert
        Assert.Equal(verwachteNaam, naam);
    }

    // Kcal-totaal passend bij aantal personen en plantaardige keuze
    [Fact]
    public void BerekenTotaalKcal_MeerderePersonen_BerekentCorrecteSom()
    {
        // Arrange
        var recept = new Recept("Spaghetti", "Spaghetti bolognese", new List<Ingredient>
        {
            new Ingredient("Spaghetti", 150, "g", 220),
            new Ingredient("Tomatensaus", 100, "g", 80),
            new Ingredient("Gehakt", 80, "g", 120, "Vega gehakt", 90)
        });

        // Act
        var totaalKcalRegulier = recept.BerekenTotaalKcal(2, gebruikPlantaardig: false);
        var totaalKcalVega = recept.BerekenTotaalKcal(2, gebruikPlantaardig: true);

        // Assert
        Assert.Equal(840, totaalKcalRegulier);
        Assert.Equal(780, totaalKcalVega);
    }

    // ReceptFactory en subklasse-instantiatie
    [Fact]
    public void ReceptFactory_MaaktJuisteSubklasseEnKenmerk()
    {
        // Arrange & Act
        var hoofdgerecht = ReceptFactory.MaakRecept(ReceptType.Hoofdgerecht, "Biefstuk", "Gebakken", new(), new(), "Chef");
        var vega = ReceptFactory.MaakRecept(ReceptType.Vegetarisch, "Salade", "Fris", new(), new(), "100% Vegan");
        var toetje = ReceptFactory.MaakRecept(ReceptType.Nagerecht, "IJs", "Koud", new(), new(), "IJskoud");

        // Assert
        Assert.IsType<HoofdgerechtRecept>(hoofdgerecht);
        Assert.Contains("Chef", hoofdgerecht.ExtraInformatie);

        Assert.IsType<VegetarischRecept>(vega);
        Assert.Contains("100% Vegan", vega.ExtraInformatie);

        Assert.IsType<NagerechtRecept>(toetje);
        Assert.Contains("IJskoud", toetje.ExtraInformatie);
    }

    // Zoeken op naam en ingrediënt (inclusief plantaardig)
    [Theory]
    [InlineData("spaghetti", true)]
    [InlineData("SPAGHETTI", true)]
    [InlineData("havermelk", true)]
    [InlineData("pizza", false)]
    public void ZoekRecept_VoldoetAanZoekterm_MatchtNaamEnIngredient(string zoekterm, bool verwachtGevonden)
    {
        // Arrange
        var recept = new Recept("Spaghetti Bolognese", "Klassieke pasta", new List<Ingredient>
        {
            new Ingredient("Koemelk", 100, "ml", 60, "Havermelk", 45)
        });

        // Act
        var resultaat = recept.VoldoetAanZoekterm(zoekterm);

        // Assert
        Assert.Equal(verwachtGevonden, resultaat);
    }

    // Tips bij bereidingsstappen
    [Fact]
    public void Bereidingsstap_TipAanwezig_HeeftTipIsTrue()
    {
        // Arrange
        var stapMetTip = new Bereidingsstap("Kook de pasta", 9, "Voeg zout toe aan het water");
        var stapZonderTip = new Bereidingsstap("Serveer", 2);

        // Assert
        Assert.True(stapMetTip.HeeftTip);
        Assert.Equal("Voeg zout toe aan het water", stapMetTip.Tip);
        Assert.False(stapZonderTip.HeeftTip);
    }

    // Toevoegen en verwijderen van recepten in repository
    [Fact]
    public void VoegReceptToe_En_VerwijderRecept_WerktCorrect()
    {
        // Arrange
        var manager = new FakeDatabase();
        var nieuwRecept = new Recept("Pannenkoeken", "Lekker bakken");

        // Act 1: Toevoegen
        manager.VoegReceptToe(nieuwRecept);
        var alleReceptenNaToevoegen = manager.GetAlleRecepten();

        // Assert 1
        Assert.Single(alleReceptenNaToevoegen);
        Assert.Equal("Pannenkoeken", alleReceptenNaToevoegen[0].Naam);

        // Act 2: Verwijderen
        bool isVerwijderd = manager.VerwijderRecept(nieuwRecept);
        var alleReceptenNaVerwijderen = manager.GetAlleRecepten();

        // Assert 2
        Assert.True(isVerwijderd);
        Assert.Empty(alleReceptenNaVerwijderen);
    }

    // Berekenen van bereidingstijden en statistieken
    [Fact]
    public void BerekenTotaleBereidingstijd_MeerdereStappen_GeeftSomVanStappenDuur()
    {
        // Arrange
        var recept = new Recept("Pasta", "Test", new(), new List<Bereidingsstap>
        {
            new Bereidingsstap("Koken", 5),
            new Bereidingsstap("Saus", 8),
            new Bereidingsstap("Mengen", 10),
            new Bereidingsstap("Serveren", 3)
        });

        // Act & Assert
        Assert.Equal(26, recept.BerekenTotaleBereidingsTijd());
    }

    [Fact]
    public void BerekenGemiddeldeBereidingsTijd_ViaFakeDatabase_GeeftJuistGemiddelde()
    {
        // Arrange
        var manager = new FakeDatabase();
        manager.VoegReceptToe(new Recept("R1", "O1", new(), new List<Bereidingsstap> { new Bereidingsstap("S1", 10) }));
        manager.VoegReceptToe(new Recept("R2", "O2", new(), new List<Bereidingsstap> { new Bereidingsstap("S2", 20) }));
        var stats = new ReceptenStatistiekService(manager);

        // Act
        var average = stats.BerekenGemiddeldeBereidingstijd();

        // Assert
        Assert.Equal(15.0f, average);
    }

    [Fact]
    public void BerekenGemiddeldeBereidingsTijd_EdgeCase_LegeRepository_GeeftNul()
    {
        // Arrange
        var manager = new FakeDatabase();
        var stats = new ReceptenStatistiekService(manager);

        // Act
        var average = stats.BerekenGemiddeldeBereidingstijd();

        // Assert
        Assert.Equal(0, average);
    }

    // JSON persistentie en foutafhandeling
    [Fact]
    public void JsonPersistentie_SlaatOpEnLaadtOpnieuwIn()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"test_recepten_{System.Guid.NewGuid()}.json");
        try
        {
            var manager1 = new ReceptenManager(tempFile);
            var nieuw = new HoofdgerechtRecept("Lasagne", "Ovenpasta", new(), new(), "Makkelijk");

            // Act
            manager1.VoegReceptToe(nieuw);

            var manager2 = new ReceptenManager(tempFile);
            var geladenRecepten = manager2.GetAlleRecepten();

            // Assert
            Assert.True(File.Exists(tempFile));
            Assert.Contains(geladenRecepten, r => r.Naam == "Lasagne" && r is HoofdgerechtRecept);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void JsonPersistentie_CorruptBestand_CrashtNietEnLaadtDefaults()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"corrupt_recepten_{System.Guid.NewGuid()}.json");
        File.WriteAllText(tempFile, "{ DIT IS ONGELDIGE JSON !!! }");

        try
        {
            // Act
            var manager = new ReceptenManager(tempFile);
            var recepten = manager.GetAlleRecepten();

            // Assert
            Assert.Equal(3, recepten.Count);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
