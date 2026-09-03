using System;

namespace Receptenboek
{
    public static class ConsoleHelper
    {
        public static string LeesTekst(string prompt, bool verplicht = true)
        {
            while (true)
            {
                Console.Write(prompt);
                string? invoer = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(invoer))
                {
                    return invoer.Trim();
                }
                if (!verplicht)
                {
                    return string.Empty;
                }
                Console.WriteLine("❌ Invoer mag niet leeg zijn. Probeer het opnieuw.");
            }
        }

        public static int LeesInt(string prompt, int min = int.MinValue, int max = int.MaxValue, int standaardWaarde = int.MinValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? invoer = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(invoer) && standaardWaarde != int.MinValue)
                {
                    return standaardWaarde;
                }

                if (int.TryParse(invoer, out int resultaat) && resultaat >= min && resultaat <= max)
                {
                    return resultaat;
                }

                Console.WriteLine($"❌ Ongeldige invoer. Voer een geheel getal in{(min != int.MinValue ? $" tussen {min} en {max}" : "")}.");
            }
        }

        public static double LeesDouble(string prompt, double min = 0.0)
        {
            while (true)
            {
                Console.Write(prompt);
                string? invoer = Console.ReadLine();

                if (double.TryParse(invoer, out double resultaat) && resultaat >= min)
                {
                    return resultaat;
                }

                Console.WriteLine($"❌ Ongeldige invoer. Voer een geldig getal in (minimaal {min}).");
            }
        }

        public static bool LeesJaNee(string prompt, bool standaard = false)
        {
            string standaardTekst = standaard ? "[J/n]" : "[j/N]";
            while (true)
            {
                Console.Write($"{prompt} {standaardTekst}: ");
                string? invoer = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(invoer))
                {
                    return standaard;
                }

                if (invoer == "j" || invoer == "ja" || invoer == "y" || invoer == "yes")
                {
                    return true;
                }
                if (invoer == "n" || invoer == "nee" || invoer == "no")
                {
                    return false;
                }

                Console.WriteLine("❌ Ongeldige invoer. Voer 'j' (ja) of 'n' (nee) in.");
            }
        }
    }
}
