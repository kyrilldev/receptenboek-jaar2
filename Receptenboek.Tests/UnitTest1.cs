using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace Receptenboek.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 40)]
    [InlineData(4, 80)]
    [InlineData(45, 900)]
    public void GetHoeveelheid_4Personen_BerekentVierVoudigeHoeveelheid(int aantalPersonen, int verwachteHoeveelheid)
    {
        var ingredient = new Ingredient("kaas", 20, "g", 80);

        var hoeveelheid = ingredient.GetHoeveelheid(aantalPersonen);

        Assert.Equal(verwachteHoeveelheid, hoeveelheid);
    }

    [Theory]
    [InlineData(false, "melk")]
    [InlineData(true, "havermelk")]
    public void GetNaam_PlantaardigAlternatief_GeeftPlantaardigAlternatief(bool gebruikPlantaardig, string verwachteNaam)
    {
        var ingredient = new Ingredient("melk", 250, "ml", kcal: 110, "havermelk", plantaardigeKcal: 90);

        var plantaardigeNaam = ingredient.GetNaam(gebruikPlantaardig);

        Assert.Equal(verwachteNaam, plantaardigeNaam);
    }

    [Fact]
    public void BerekenTotaalKcalorien_SomAlleIngredienten_TotaalVermenigvuldigtMetTwee()
    {
        var recept = new Recept("Spaghetti", "Spaghetti met tomaten saus, gehakt en groenten", [new Ingredient("Spaghetti", 150, "g", 220), new Ingredient("Tomaten Saus", 100, "g", 80), new Ingredient("Gehakt en groenten mix", 80, "g", 120)]);

        var totaalKcalorien = recept.BerekenTotaalKcal(2, false);

        Assert.Equal(840, totaalKcalorien);
    }

    [Fact]
    public void BerekenTotaleBereidingstijd_MeerdereStappen_GeeftSomVanStappenDuur()
    {
        var recept = new Recept("Spaghetti", "Spaghetti met tomaten saus, gehakt en groenten",
        new List<Ingredient>
            {
                new Ingredient("Spaghetti", 150, "g", 220),
                new Ingredient("Tomaten Saus", 100, "g", 80),
                new Ingredient("Gehakt en groenten mix", 80, "g", 120)
            },
        new List<Bereidingsstap>
            {
                new Bereidingsstap("Kook de spaghetti", 5),
                new Bereidingsstap("Doe de saus in een pan en breng aan de kook", 8),
                new Bereidingsstap("Voeg samen in één pan en kook verder", 10),
                new Bereidingsstap("Verspreid over een bord en geniet!", 3)
            });

        Assert.Equal(26, recept.BerekenTotaleBereidingsTijd());


    }

    [Fact]
    public void BerekenGemiddeldeBereidingsTijd_Test_DeJuisteBeredingstijdViaNepDatabase()
    {
        //per recept in receptenboek
        var manager = new FakeDatabase();
        manager.VoegReceptToe(new Recept("Stampot", "Gestampte aardappels met groenten en vlees", new List<Ingredient>
        {
            new Ingredient("Aardappels", 2, "stuks", 120),
            new Ingredient("Andijvie", 2, "stuks", 180),
            new Ingredient("Spekjes", 1, "pakje", 400)
        },
        new List<Bereidingsstap>
        {
            new Bereidingsstap("Stamp de aardappels", 10),
            new Bereidingsstap("Schil de andijvie", 4),
            new Bereidingsstap("Mix de 2 en kook door", 5)
        }));
        // Variatie 1: Pasta Bolognese
        manager.VoegReceptToe(new Recept("Pasta Bolognese", "Klassieke Italiaanse pasta met een rijke gehaktsaus", new List<Ingredient>
        {
            new Ingredient("Spaghetti", 400, "gram", 350),
            new Ingredient("Rundergehakt", 500, "gram", 450),
            new Ingredient("Gezeefde tomaten", 1, "fles", 120),
            new Ingredient("Ui", 1, "stuk", 60)
        },
        new List<Bereidingsstap>
        {
            new Bereidingsstap("Snipper de ui en fruit deze aan", 3),
            new Bereidingsstap("Rul het gehakt en voeg de tomatensaus toe", 12),
            new Bereidingsstap("Kook de spaghetti beetgaar", 9),
            new Bereidingsstap("Meng de saus met de pasta en serveer", 2)
        }));

        // Variatie 2: Groene Smoothie
        manager.VoegReceptToe(new Recept("Groene Smoothie", "Een frisse en gezonde energieshot voor de ochtend", new List<Ingredient>
        {
            new Ingredient("Banaan", 1, "stuk", 100),
            new Ingredient("Verse spinazie", 100, "gram", 30),
            new Ingredient("Havermelk", 250, "ml", 120),
            new Ingredient("Chiazaad", 1, "eetlepel", 60)
        },
        new List<Bereidingsstap>
        {
            new Bereidingsstap("Pel de banaan en breek in stukken", 1),
            new Bereidingsstap("Voeg alle ingrediënten toe aan de blender", 2),
            new Bereidingsstap("Blend op hoge snelheid tot een gladde drank", 2)
        }));
        var stats = new ReceptenStatistiekService(manager);

        var average = stats.BerekenGemiddeldeBereidingstijd();

        Assert.Equal(16.666, average, 2);
    }

    [Fact]
    public void BerekenGemiddeldeBereidingsTijd_EdgeCase_LegeRepository()
    {
        var manager = new FakeDatabase();

        var stats = new ReceptenStatistiekService(manager);

        var average = stats.BerekenGemiddeldeBereidingstijd();

        Assert.Equal(0, average);
    }
}
