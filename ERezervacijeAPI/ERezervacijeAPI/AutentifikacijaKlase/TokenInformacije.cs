using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.AutentifikacijaKlase
{
    public class TokenInformacije
    {
        public string? Token { get; set; }
        public bool IsLogiran { get; set; }
        public bool IsGost { get; set; }
        public bool IsManager { get; set; }
        public bool IsHostesa { get; set; }
        public Gost? Gost { get; set; }
        public Uposlenik? Uposlenik { get; set; }
        public int Greska { get; set; }

    }
}
