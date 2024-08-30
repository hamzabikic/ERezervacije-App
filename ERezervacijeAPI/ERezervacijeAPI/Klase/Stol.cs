using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Stol")]
    public class Stol
    {
        [Key]
        public int StolID { get; set; }
        public int BrojStola { get; set; }  
        public bool JeUnutra { get; set; }
        public bool JePusackaZona { get; set; }
        public int BrojStolica { get; set; }
    }
}
