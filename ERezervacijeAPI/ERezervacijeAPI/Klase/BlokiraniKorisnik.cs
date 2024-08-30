using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("BlokiraniKorisnik")]
    public class BlokiraniKorisnik
    {
        public int BlokiraniKorisnikId { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
    }
}
