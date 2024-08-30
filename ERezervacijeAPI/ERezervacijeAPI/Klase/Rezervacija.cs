using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Rezervacija")]
    public class Rezervacija
    {
        [Key]
        public int RezervacijaID { get; set; }
        public DateTime DatumVrijeme{ get; set; }
        public int Trajanje { get; set; }
        public string Komentar { get; set; }
        public string PosebneZelje { get; set; }
        public bool Odobrena { get; set; }
        public bool Preuzeta { get; set; }
        [ForeignKey("Gost")]
        public int? GostID { get; set; }
        public Gost? Gost { get; set; }
        public string? ImePrezime { get; set; }
        public string? BrojTelefona { get; set; }
        public bool Ponistena { get; set; }
        public bool Otkazana { get; set; }
    }
}
