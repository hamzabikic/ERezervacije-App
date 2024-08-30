using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Popust")]
    public class Popust
    {
        [Key]
        public int PopustID { get; set; }
        public int Procenat { get; set; }
        public string Razlog { get; set; }
    }
}
