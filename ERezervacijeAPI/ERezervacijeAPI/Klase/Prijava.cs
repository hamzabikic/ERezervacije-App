using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Prijava")]
    public class Prijava
    {
        [Key]
        public int PrijavaID { get; set; }
        [ForeignKey("Korisnik")]
        public int KorisnikKorisnikID { get; set; }
        public Korisnik Korisnik { get; set; }
        public DateTime DatumVrijeme { get; set; }
        public string Token { get; set; }
        public bool IsGost { get; set; }
        public bool IsManager { get; set; }
        public bool IsHostesa { get; set; }

    }
}
