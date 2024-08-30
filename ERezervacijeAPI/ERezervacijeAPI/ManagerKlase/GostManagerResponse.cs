using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.ManagerKlase
{
    public class KorisnikManagerReponse
    {
        public int KorisnikID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public int GradID { get; set; }
        public virtual Grad Grad { get; set; }
    }
    public class GostManagerResponse :KorisnikManagerReponse
    {
        public int BrojRezervacija { get; set; }
        public DateTime DatumRegistracije { get; set; }
    }
}
