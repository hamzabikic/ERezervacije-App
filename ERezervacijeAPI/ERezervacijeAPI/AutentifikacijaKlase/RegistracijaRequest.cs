using ERezervacijeAPI.Klase;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.AutentifikacijaKlase
{
    public class RegistracijaRequest
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public bool Aktivan { get; set; }
        public int GradID { get; set; }
    }
}
