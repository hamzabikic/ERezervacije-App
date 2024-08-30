using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("PromjenaLozinke")] 
    public class PromjenaLozinke
    {
        [Key]
        public int PromjenaLozinkeID { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikID { get; set; }
        public Korisnik Korisnik { get; set; }
        public DateTime VrijemePrvogIzdavanja { get; set; }
        public DateTime ValidnaDo { get; set; }
        public string Key { get; set; }
        public int BrojPromjena { get; set; }
    }
}
