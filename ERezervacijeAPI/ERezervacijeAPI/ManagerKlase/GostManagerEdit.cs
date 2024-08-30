namespace ERezervacijeAPI.ManagerKlase
{
    public class KorisnikManagerEdit
    {
        public int KorisnikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int GradId { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string BrojTelefona { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
    public class HostesaManagerEdit:KorisnikManagerEdit
    {
        public DateTime DatumZaposlenja { get; set; }
        public string StrucnaSprema { get; set; }
    }
}
