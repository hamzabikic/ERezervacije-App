using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("ZabranaStol")]
    public class ZabranaStol
    {
        [Key]
        public int ZabranaStolId { get; set; }
        [ForeignKey(nameof(Zabrana))]
        public int ZabranaId { get; set; }
        public Zabrana Zabrana { get; set; }
        [ForeignKey(nameof(Stol))]
        public int StolId { get; set; }
        public Stol Stol { get; set; }
    }
}
