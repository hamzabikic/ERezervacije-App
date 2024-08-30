using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Gost_Popust")]
    public class GostPopust
    {
        [Key]
        public int GostPopustID { get; set; }
        [ForeignKey("Gost")]
        public int GostID { get; set; }
        public Gost Gost { get; set; }
        [ForeignKey("Popust")]
        public int PopustID { get; set; }
        public Popust Popust { get; set; }
        public bool Iskoristen { get; set; }
    }
}
