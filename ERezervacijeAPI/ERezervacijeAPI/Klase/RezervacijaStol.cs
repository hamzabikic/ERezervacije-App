using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Rezervacija_Stol")]
    public class RezervacijaStol
    {
        [Key]
        public int RezervacijaStolId { get; set; }
        [ForeignKey("Rezervacija")]
        public int RezervacijaID { get; set; }
        public Rezervacija Rezervacija { get; set; }
        [ForeignKey("Stol")]
        public int StolID { get; set; }
        public Stol Stol { get; set; }

    }
}
