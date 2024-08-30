using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("NutritivneVrijednosti")]
    public class NutritivneVrijednosti
    {
        [Key]
        public int NutritivneId { get; set; }
        public float Kalorije { get; set; }
        public float Ugljikohidrati { get; set; }
        public float Masti { get; set; }
        public float Proteini { get; set; }
        public float Seceri { get; set; }
    }
}
