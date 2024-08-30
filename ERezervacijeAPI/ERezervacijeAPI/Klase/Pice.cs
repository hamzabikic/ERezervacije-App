using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Pice")]
    public class Pice :Proizvod
    {
        public float Kolicina { get; set; }
    }
}
