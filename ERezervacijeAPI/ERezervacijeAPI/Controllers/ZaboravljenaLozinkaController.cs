using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.KorisnikKlase;
using ERezervacijeAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    public class ZaboravljenaLozinkaController : ControllerBase
    {
        private readonly DBConnection db;
        private readonly MyAuthService auth;
        private readonly SmtpService smtp;
        public ZaboravljenaLozinkaController(DBConnection _db, MyAuthService _auth, SmtpService _smtp )
        {
            db = _db;
            auth = _auth;
            smtp = _smtp;
        }
        [HttpGet("ZaboravljenaLozinkaSlanje")]
        public async Task<LozinkaResponse> ZaboravljenaLozinkaSlanje (string email )
        {
            var prijava = await auth.isLogiran();
            if (prijava.JePrijavljen) return new LozinkaResponse
            {
                Promijenjena = false,
                Greska = ""
            };
            var korisnik = await db.Korisnici.FirstOrDefaultAsync(k => k.Email == email);
            if (korisnik == null) return new LozinkaResponse
            {
                Promijenjena = false,
                Greska = "Email nije pronadjen u sistemu!"
            };
            var promjena = await db.PromjeneLozinke.FirstOrDefaultAsync(pl => pl.KorisnikID == korisnik!.KorisnikID);
            if(promjena == null)
            {
                var nova = new PromjenaLozinke
                {
                    KorisnikID = korisnik!.KorisnikID,
                    BrojPromjena = 1,
                    ValidnaDo = DateTime.Now.AddMinutes(10),
                    VrijemePrvogIzdavanja = DateTime.Now,
                    Key = UserKeyGenerator.GenerisiKey()
                };
                db.PromjeneLozinke.Add(nova);
                db.SaveChanges();
                var poruka = $"<h3 style='display:block;color:black' >Promjena lozinke</h3>" +
                     $"<a href = '{AddressesHelper.WebPageAddress}#/nova-lozinka/{nova.Key}'> Kliknite ovdje za promjenu lozinke</a>" +
                     $"<h4 style='display:block;color:black;'> Link za promjenu lozinke vrijedi 10 minuta.</h4>";
                await smtp.sendEmail("Promjena lozinke", poruka, korisnik.Email);
                return new LozinkaResponse
                {
                    Promijenjena = true,
                    Greska = ""
                };
            }
            else
            {
                if(DateTime.Now > promjena.VrijemePrvogIzdavanja.AddHours(24))
                {
                    promjena.BrojPromjena = 0;
                    db.SaveChanges();
                }
                if(promjena.BrojPromjena==10)
                {
                    return new LozinkaResponse
                    {
                        Promijenjena = false,
                        Greska = "Trenutno nije moguce slanje zahtjeva!"
                    }; ;
                }
                if(promjena.BrojPromjena==0)
                {
                    promjena.VrijemePrvogIzdavanja = DateTime.Now;
                }
                promjena.BrojPromjena = promjena.BrojPromjena + 1;
                promjena.ValidnaDo = DateTime.Now.AddMinutes(10);
                promjena.Key = UserKeyGenerator.GenerisiKey();
                db.SaveChanges();
                var poruka = $"<h3 style='display:block;color:black' >Promjena lozinke</h3>" +
                    $"<a href = '{AddressesHelper.WebPageAddress}#/nova-lozinka/{promjena.Key}'> Kliknite ovdje za promjenu lozinke</a>" +
                    $"<h4 style='display:block;color:black;'> Link za promjenu lozinke vrijedi 10 minuta.</h4>";
                await smtp.sendEmail("Promjena lozinke", poruka, korisnik.Email);
                return new LozinkaResponse
                {
                    Promijenjena = true,
                    Greska = ""
                };
            }
        }
        [HttpPost("PromjenaZaboravljeneLozinke")]
        public async Task<LozinkaResponse> PromjenaZaboravljeneLozinke (PromjenaLozinkeRequest req)
        {
            var prijava = await auth.isLogiran();
            if (prijava.JePrijavljen) return new LozinkaResponse { Promijenjena = false, Greska = "" };
            if (req.NovaLozinka.Length < 8) return new LozinkaResponse
            {
                Promijenjena = false,
                Greska = "Lozinka mora sadrzavati minimalno 8 karaktera!"
            };
            var promjena = await db.PromjeneLozinke.FirstOrDefaultAsync(pl => pl.Key == req.Key);
            if (promjena == null) return new LozinkaResponse { Promijenjena = false, Greska = "Kljuc nije pronadjen u sistemu!" };
            if(DateTime.Now > promjena.ValidnaDo)
            {
                    return new LozinkaResponse
                    {
                        Promijenjena = false,
                        Greska = "Kljuc za promjenu lozinke je istekao!"
                    };
            }
            var korisnik = await db.Korisnici.FindAsync(promjena.KorisnikID);
            if(korisnik!.Password == req.NovaLozinka)
            {
                return new LozinkaResponse
                {
                    Promijenjena = false,
                    Greska = "Unesena lozinka se trenutno koristi!"
                };
            }
            korisnik.Password = req.NovaLozinka;
            promjena.ValidnaDo = DateTime.Now;
            db.SaveChanges();
            return new LozinkaResponse { Promijenjena = true, Greska = "" };
        }
        [HttpGet("PostojiKljuc")]
        public async Task<bool> PostojiKljuc(string key)
        {
            var prijava = await auth.isLogiran();
            if (prijava.JePrijavljen) return false;
            var promjena = await db.PromjeneLozinke.FirstOrDefaultAsync(pl => pl.Key == key);
            if (promjena == null) return false;
            if (DateTime.Now > promjena.ValidnaDo) return false;
            return true;
        }
    }
}
