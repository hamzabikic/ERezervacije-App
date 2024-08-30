using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.RecenzijeKlase
{
    public class RecenzijaManagerResponse
    {
        public int RecenzijaId { get; set; }
        public int RezervacijaId { get; set; }
        public string Datum { get; set; }
        public string Vrijeme { get; set; }
        public string ImePrezime { get; set; }
        public string BrojTelefona { get; set; }
        public int Ocjena { get; set; }
    }

    public class RecenzijaInfoManager :RecenzijaManagerResponse
    {
        public string Komentar { get; set; }
        public List<Stol> Stolovi { get; set; }
        public string PosebneZelje { get; set; }
    }
}
