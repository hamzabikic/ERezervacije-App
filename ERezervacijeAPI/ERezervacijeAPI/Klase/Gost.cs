using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERezervacijeAPI.Klase
{
    [Table("Gost")]
    public class Gost :Korisnik
    {
        [JsonIgnore]
        public DateTime DatumRegistracije { get; set; }
        [JsonIgnore]
        public int BrojRezervacija { get; set; }
    }
}
