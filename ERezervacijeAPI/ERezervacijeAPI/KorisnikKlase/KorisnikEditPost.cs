﻿using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.KorisnikKlase
{
    public class KorisnikEditPost
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public int GradID { get; set; }
    }
}
