using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Recenzija")]
    public class Recenzija 
    {
        public int RecenzijaId { get; set; }
        [ForeignKey("Rezervacija")]
        public int RezervacijaId { get; set; }
        public Rezervacija Rezervacija { get; set; }
        public int Ocjena { get; set; }
        public string Komentar { get; set; }
    }
}
