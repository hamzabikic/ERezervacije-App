using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Narudzba")]
    public class Narudzba 
    {
        [Key]
        public int NarudzbaId { get; set; }
        public bool Odobrena { get; set; }
        public bool Odbijena { get; set; }
        public string Zahtjevi { get; set; }
        [ForeignKey(nameof(Rezervacija))]
        public int RezervacijaId { get; set; }
        public Rezervacija Rezervacija { get; set; }
    }
}
