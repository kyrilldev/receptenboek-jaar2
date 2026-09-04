namespace Receptenboek.Domain
{
    public class Bereidingsstap
    {
        public string Beschrijving { get; set; }
        public int DuurMinuten { get; set; }
        public string? Tip { get; set; }

        public bool HeeftTip => !string.IsNullOrWhiteSpace(Tip);

        public Bereidingsstap(string beschrijving, int duurMinuten, string? tip = null)
        {
            Beschrijving = beschrijving;
            DuurMinuten = duurMinuten;
            Tip = tip;
        }
    }
}
