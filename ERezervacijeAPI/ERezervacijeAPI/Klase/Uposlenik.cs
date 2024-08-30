using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERezervacijeAPI.Klase
{
    [Table("Uposlenik")]
    public class Uposlenik :Korisnik
    {
        [JsonIgnore]
        public DateTime DatumZaposlenja { get; set; }
        [JsonIgnore]
        public string StrucnaSprema { get; set; }
    }
}
