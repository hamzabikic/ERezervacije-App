using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.RezervacijaKlase
{
    public class RezervacijaGostEdit
    {
        public int RezervacijaId { get; set; }
        public DateTime Datum{ get; set; }
        public int Vrijeme { get; set; }
        public int Trajanje { get; set; }
        public string PosebneZelje { get; set; }
        public List<Stol> Stolovi { get; set; }
        public bool Ponistena { get; set; }
        public string Komentar { get; set; }
    }
}
