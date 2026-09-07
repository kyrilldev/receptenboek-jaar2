using System.Collections.Generic;
using Receptenboek.Domain;
using Receptenboek.Application.Services;

namespace Receptenboek.Tests;

public class UnitTest1
{
    // =========================================================================
    // AC-07.1: Schaling van hoeveelheden op basis van aantal personen
    // =========================================================================
    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 40)]
    [InlineData(4, 80)]
    [InlineData(45, 900)]
    public void AC07_1_GetHoeveelheid_SchaaltMetAantalPersonen(int aantalPersonen, int verwachteHoeveelheid)
    {
        // Arrange
        var ingredient = new Ingredient("kaas", 20, "g", 80);

        // Act
        var hoeveelheid = ingredient.GetHoeveelheid(aantalPersonen);

        // Assert
        Assert.Equal(verwachteHoeveelheid, hoeveelheid);
    }

    // =========================================================================
    // AC-08.1 & AC-08.2: Plantaardig alternatief selectie
    // =========================================================================
    [Theory]
    [InlineData(false, "melk")]
    [InlineData(true, "havermelk")]
    public void AC08_1_GetNaam_PlantaardigAlternatief_GeeftGekozenVariant(bool gebruikPlantaardig, string verwachteNaam)
    {
        // Arrange
        var ingredient = new Ingredient("melk", 250, "ml", kcal: 110, "havermelk", plantaardigeKcal: 90);

        // Act
        var naam = ingredient.GetNaam(gebruikPlantaardig);

        // Assert
        Assert.Equal(verwachteNaam, naam);
    }

    // =========================================================================
    // AC-09.1: Kcal-totaal passend bij aantal personen en plantaardige keuze
    // =========================================================================
    [Fact]
    public void AC09_1_BerekenTotaalKcal_MeerderePersonen_BerekentCorrecteSom()
    {
        // Arrange
        var recept = new Recept("Spaghetti", "Spaghetti bolognese", new List<Ingredient>
        {
            new Ingredient("Spaghetti", 150, "g", 220),
            new Ingredient("Tomatensaus", 100, "g", 80),
            new Ingredient("Gehakt", 80, "g", 120, "Vega gehakt", 90)
        });

        // Act (2 personen, regulier vlees)
        var totaalKcalRegulier = recept.BerekenTotaalKcal(2, gebruikPlantaardig: false);
        // Act (2 personen, vega alternatief: 220 + 80 + 90 = 390 * 2 = 780)
        var totaalKcalVega = recept.BerekenTotaalKcal(2, gebruikPlantaardig: true);

        // Assert
        Assert.Equal(840, totaalKcalRegulier);
        Assert.Equal(780, totaalKcalVega);
    }

    // =========================================================================
    // AC-05.1: Receptvarianten via Factory en kenmerken
    // =========================================================================
    [Fact]
    public void AC05_1_ReceptFactory_MaaktJuisteSubklasseEnKenmerk()
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

    // =========================================================================
    // AC-06.1 & AC-06.2: Zoeken op naam en ingrediënt (inclusief plantaardig)
    // =========================================================================
    [Theory]
    [InlineData("spaghetti", true)]
    [InlineData("SPAGHETTI", true)]
    [InlineData("havermelk", true)]
    [InlineData("pizza", false)]
    public void AC06_ZoekRecept_VoldoetAanZoekterm_MatchtNaamEnIngredient(string zoekterm, bool verwachtGevonden)
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

    // =========================================================================
    // AC-10.1: Tips bij bereidingsstappen
    // =========================================================================
    [Fact]
    public void AC10_1_Bereidingsstap_TipAanwezig_HeeftTipIsTrue()
    {
        // Arrange
        var stapMetTip = new Bereidingsstap("Kook de pasta", 9, "Voeg zout toe aan het water");
        var stapZonderTip = new Bereidingsstap("Serveer", 2);

        // Assert
        Assert.True(stapMetTip.HeeftTip);
        Assert.Equal("Voeg zout toe aan het water", stapMetTip.Tip);
        Assert.False(stapZonderTip.HeeftTip);
    }

    // =========================================================================
    // AC-13.1 & AC-14.1: Toevoegen en verwijderen (CRUD) in repository
    // =========================================================================
    [Fact]
    public void AC13_1_En_AC14_1_VoegReceptToe_En_VerwijderRecept_WerktCorrect()
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

    // =========================================================================
    // Service- & Berekeningstests (Test Doubles)
    // =========================================================================
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
}
