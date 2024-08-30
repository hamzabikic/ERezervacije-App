using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Zabrana")]
    public class Zabrana
    {
        [Key]
        public int ZabranaId { get; set; }
        public DateTime PocetakVrijeme { get; set; }
        public DateTime KrajVrijeme { get; set; }
    }
}
