using ERezervacijeAPI.Klase;
using Microsoft.EntityFrameworkCore;

namespace ERezervacijeAPI.Data
{
    public class DBConnection:DbContext
    {
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Grad> Gradovi { get; set; }
        public DbSet<BlokiraniKorisnik> BlokiraniKorisnici { get; set; }
        public DbSet<Uposlenik> Uposlenici { get; set; }
        public DbSet<Hostesa> Hostese { get; set; }
        public DbSet<Manager> Manageri { get; set; }
        public DbSet<Kuhar> Kuhari { get; set; }
        public DbSet<Rezervacija> Rezervacije { get; set; }
        public DbSet<Narudzba> Narudzbe { get; set; }
        public DbSet<Kategorija> Kategorije { get; set; }
        public DbSet<NutritivneVrijednosti> NutritivneVrijednosti { get; set; }
        public DbSet<Proizvod> Proizvodi { get; set; }
        public DbSet<Jelo> Jela { get; set; }
        public DbSet<Pice> Pica { get; set; }
        public DbSet<NarudzbaProizvod> NarudzbeProizvodi { get; set; }
        public DbSet<Stol> Stolovi { get; set; }
        public DbSet<RezervacijaStol> RezervacijeStolovi { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<Gost> Gosti { get; set; }
        public DbSet<Popust> Popusti { get; set; }
        public DbSet<GostPopust> GostiPopusti { get; set; }
        public DbSet<Prijava> Prijave { get; set; }
        public DbSet<KorisnikEmail> KorisnikEmail { get; set; }
        public DbSet<KorisnikTelefon> KorisnikTelefon { get; set; }
        public DbSet<PromjenaLozinke> PromjeneLozinke { get; set; }
        public DbSet<Zabrana> Zabrane { get; set; }
        public DbSet<ZabranaStol> ZabranaStol { get; set; }

        public DBConnection(DbContextOptions options) :base(options)
        {

        }
    }
}
