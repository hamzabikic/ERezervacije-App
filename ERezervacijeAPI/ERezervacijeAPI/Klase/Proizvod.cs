using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Proizvod")]
    public class Proizvod
    {
        [Key]
        public int ProizvodID { get; set; }
        public string Naziv { get; set; }
        public float Cijena { get; set; }
        [ForeignKey("Kategorija")]
        public int KategorijaID { get; set; }
        public Kategorija Kategorija { get; set; }
        [ForeignKey(nameof(Nutritivne))]
        public int NutritivneId { get; set; }
        public NutritivneVrijednosti Nutritivne { get; set; }
    }
}
