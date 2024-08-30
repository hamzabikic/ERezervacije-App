using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Klase;
using Microsoft.AspNetCore.Mvc;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestniPodaciController
    {
        private readonly DBConnection db;
        public TestniPodaciController(DBConnection _db)
        {
            db = _db;
        }
        [HttpPost]
        public void generisiTestnePodatke()
        {
            var opstina1 = new Grad { Naziv = "Sarajevo", Drzava = "BiH", PTT = 71000 };
            var opstina2 = new Grad { Naziv = "Mostar", Drzava = "BiH", PTT = 88000 };
            var opstina3 = new Grad { Naziv = "Banja Luka", Drzava = "BiH", PTT = 78000 };
            db.Gradovi.AddRange(opstina1, opstina2, opstina3);
            db.SaveChanges();
            var gost = new Gost
            {
                Ime = "Gost",
                Prezime = "Gost",
                Email = "gost@mail.com",
                BrojTelefona = "+38760000000",
                DatumRodjenja = new DateTime(2001, 1, 1),
                DatumRegistracije = DateTime.Now,
                Password = "testtest",
                GradID = opstina1.GradID,
                Aktivan = true,
                Slika = "profilne-slike/empty.jpg",
                BrojRezervacija = 0,
                Username = "gost"
            };
            db.Gosti.Add(gost);
            db.SaveChanges();
            var korisnikTelefon = new KorisnikTelefon
            {
                BrojIzdavanja = 1,
                VrijemeIsteka = DateTime.Now.AddMinutes(2),
                Verifikovan = true,
                Key = UserKeyGenerator.GenerisiPhoneKey(),
                GostId = gost.KorisnikID,
                VrijemePrvogIzdavanja = DateTime.Now
            };
            db.KorisnikTelefon.Add(korisnikTelefon);
            var korisnikEmail = new KorisnikEmail
            {
                BrojIzdavanja = 1,
                VrijemeIsteka = DateTime.Now.AddMinutes(30),
                Verifikovan = true,
                Key = UserKeyGenerator.GenerisiKey(),
                GostId = gost.KorisnikID,
                VrijemePrvogIzdavanja = DateTime.Now
            };
            db.KorisnikEmail.Add(korisnikEmail);
            db.SaveChanges();
            var hostesa = new Hostesa
            {
                Ime = "Hostesa",
                Prezime = "Hostesa",
                DatumRodjenja = new DateTime(1999, 5, 1),
                DatumZaposlenja = DateTime.Now,
                Aktivan = true,
                GradID = opstina2.GradID,
                BrojTelefona = "+38761111111",
                Email = "hostesa@mail.com",
                Password = "testtest",
                Slika = "profilne-slike/empty.jpg",
                StrucnaSprema = "diplomirani ekonomista",
                Username = "hostesa"
            };
            db.Hostese.Add(hostesa);
            var manager = new Manager
            {
                Ime = "Manager",
                Prezime = "Manager",
                DatumRodjenja = new DateTime(1997, 6, 21),
                DatumZaposlenja = DateTime.Now,
                Aktivan = true,
                GradID = opstina3.GradID,
                BrojTelefona = "+38762222222",
                Email = "manager@mail.com",
                Password = "testtest",
                Slika = "profilne-slike/empty.jpg",
                StrucnaSprema = "diplomirani ekonomista",
                Username = "manager"
            };
            db.Manageri.Add(manager);
            db.SaveChanges();
            db.Stolovi.AddRange(
                new Stol
                {
                    BrojStola = 1,
                    BrojStolica = 4,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 2,
                    BrojStolica = 4,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 3,
                    BrojStolica = 4,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 4,
                    BrojStolica = 2,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 5,
                    BrojStolica = 2,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 6,
                    BrojStolica = 2,
                    JePusackaZona = false,
                    JeUnutra = true
                },
                new Stol
                {
                    BrojStola = 7,
                    BrojStolica = 4,
                    JePusackaZona = true,
                    JeUnutra = true
                },
                 new Stol
                 {
                     BrojStola = 8,
                     BrojStolica = 4,
                     JePusackaZona = true,
                     JeUnutra = true
                 },  new Stol
                 {
                     BrojStola = 9,
                     BrojStolica = 4,
                     JePusackaZona = true,
                     JeUnutra = true
                 },
                  new Stol
                  {
                      BrojStola = 10,
                      BrojStolica = 2,
                      JePusackaZona = true,
                      JeUnutra = true
                  },
                   new Stol
                   {
                       BrojStola = 11,
                       BrojStolica = 2,
                       JePusackaZona = true,
                       JeUnutra = true
                   },
                    new Stol
                    {
                        BrojStola = 12,
                        BrojStolica = 2,
                        JePusackaZona = true,
                        JeUnutra = true
                    },
                     new Stol
                     {
                         BrojStola = 13,
                         BrojStolica = 4,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 14,
                         BrojStolica = 4,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 15,
                         BrojStolica = 4,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 16,
                         BrojStolica = 4,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 17,
                         BrojStolica = 2,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 18,
                         BrojStolica = 2,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 19,
                         BrojStolica = 2,
                         JePusackaZona = true,
                         JeUnutra = false
                     },
                     new Stol
                     {
                         BrojStola = 20,
                         BrojStolica = 2,
                         JePusackaZona = true,
                         JeUnutra = false
                     }
                );
            db.SaveChanges();

        }

    }
}
