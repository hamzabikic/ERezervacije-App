using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.ManagerKlase
{
    public class ZabranaStolaPost
    {
        public List<int> Stolovi { get; set; }
        public DateTime PocetakVrijeme { get; set; }
        public DateTime KrajVrijeme { get; set; }
        
    }
}
