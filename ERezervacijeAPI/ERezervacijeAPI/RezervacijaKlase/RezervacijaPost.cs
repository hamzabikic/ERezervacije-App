namespace ERezervacijeAPI.RezervacijaKlase
{
    public class RezervacijaPost
    {
        public DateTime DatumVrijeme{ get; set; }
        public int Trajanje { get; set; }
        public string PosebneZelje { get; set; }
        public int GostID { get; set; }
        public List<int> Stolovi { get; set; }
    }
}
