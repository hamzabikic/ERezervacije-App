namespace ERezervacijeAPI.RezervacijaKlase
{
    public class RezervacijaGostResponse
    {
        public int RezervacijaID { get; set; }
        public string DatumRezervacije { get; set; }
        public string VrijemeRezervacije { get; set; }
        public List<int> Stolovi { get; set; }
        public bool Odobrena { get; set; }
        public bool Preuzeta { get; set; }
        public bool Recenzirano { get; set; }
        public string Komentar { get; set; }
        public bool Ponistena { get; set; }
        public bool Otkazana { get; set; }
    }
}
