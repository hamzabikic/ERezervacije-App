using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Kategorija")]
    public class Kategorija
    {
        [Key]
        public int KategorijaID { get; set; }
        public string NazivKategorije { get; set; }
    }
}
