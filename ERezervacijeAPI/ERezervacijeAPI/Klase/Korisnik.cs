using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERezervacijeAPI.Klase
{
    [Table("Korisnik")]
    public class Korisnik
    {
        [Key]
        public int KorisnikID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public string BrojTelefona { get; set; }
        [JsonIgnore]
        public DateTime DatumRodjenja { get; set; }
        [JsonIgnore]
        public string? Slika { get; set; }
        [JsonIgnore]
        public bool Aktivan { get; set; }
        [JsonIgnore]
        [ForeignKey("Grad")]
        public int GradID { get; set; }
        [JsonIgnore]

        public virtual Grad Grad { get; set; }
        [JsonIgnore]
        public bool isGost => (this as Gost) == null ? false : true;
        [JsonIgnore]
        public bool isHostesa => (this as Hostesa) == null ? false : true;
        [JsonIgnore]
        public bool isManager => (this as Manager) == null ? false : true;
        [JsonIgnore]
        public bool isKuhar => (this as Manager) == null ? false : true;
        
    }
}
