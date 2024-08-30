namespace ERezervacijeAPI.RezervacijaKlase
{
    public class ManuelnaRezPost
    {
        public DateTime DatumVrijeme { get; set; }
        public int Trajanje { get; set; }
        public string PosebneZelje { get; set; }
        public string ImePrezime { get; set; }
        public string BrojTelefona { get; set; }
        public List<int> Stolovi { get; set; }
    }
}
