using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    [Table("Jelo")]
    public class Jelo : Proizvod
    {
        public string Sastojci { get; set; }

    }
}
