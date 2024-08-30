using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{

    [Table("Narudzba_Proizvod")]
    public class NarudzbaProizvod
    {
        [Key]
        public int NarudzbaProizvodID { get; set; }
        public int Kolicina { get; set; }
        [ForeignKey("Narudzba")]
        public int NarudzbaID { get; set; }
        public Narudzba Narudzba { get; set; }
        [ForeignKey(nameof(Proizvod))]
        public int ProizvodID { get; set; }
        public Proizvod Proizvod { get; set; }
    }
}
