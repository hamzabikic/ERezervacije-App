using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Grad")]
    public class Grad
    {
        [Key]
        public int GradID { get; set; }
        public string Naziv { get; set; }
        public int PTT { get; set; }
        public string Drzava { get; set; }
    }
}
