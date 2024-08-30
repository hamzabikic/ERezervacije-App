using ERezervacijeAPI.Klase;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.KorisnikKlase
{
    public class KorisnikResponse
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public DateTime  DatumRodjenja { get; set; }
        public int GradID { get; set; }

        public  Grad Grad { get; set; }
    }
    public class GostResponse :KorisnikResponse
    {
        public int BrojRezervacija { get; set; }
        public string DatumRegistracije { get; set; }
        public bool Aktivan { get; set; }
        public bool VerifikovanEmail { get; set; }
        public bool VerifikovanTelefon { get; set; }
    }
}
