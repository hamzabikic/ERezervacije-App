using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERezervacijeAPI.Klase
{
    public class KorisnikTelefon
    {
        public int KorisnikTelefonId { get; set; }
        [ForeignKey("Gost")]
        public int GostId { get; set; }
        public Gost Gost { get; set; }
        public bool Verifikovan { get; set; }
        public string Key { get; set; }
        public DateTime VrijemePrvogIzdavanja { get; set; }
        public DateTime VrijemeIsteka { get; set; }
        public int BrojIzdavanja { get; set; }

    }
}
