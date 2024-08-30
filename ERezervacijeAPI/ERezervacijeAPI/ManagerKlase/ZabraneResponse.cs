using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.ManagerKlase
{
    public class ZabranaResponse
    {
        public int ZabranaId { get; set; }
        public List<Stol> Stolovi { get; set; }
        public string PocetakDatum { get; set; }
        public string VrijemeOd { get; set; }
        public string KrajDatum { get; set; }
        public string VrijemeDo { get; set; }
    }
}
