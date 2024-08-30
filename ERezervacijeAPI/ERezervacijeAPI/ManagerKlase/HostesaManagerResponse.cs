using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.ManagerKlase
{
    public class HostesaManagerResponse:KorisnikManagerReponse
    {
        public DateTime DatumZaposlenja { get; set; }
        public string StrucnaSprema { get; set; }

    }
}
