using ERezervacijeAPI.AutentifikacijaKlase;
using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.KorisnikKlase;
using ERezervacijeAPI.ManagerKlase;
using ERezervacijeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using static System.Net.WebRequestMethods;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    public class AutentifikacijaController
    {
        private readonly DBConnection DB;
        private readonly MyAuthService MyAuth;
        private readonly SmtpService Smtp;
        public AutentifikacijaController(DBConnection _DB,MyAuthService _MyAuth, SmtpService _service)
        {
            DB = _DB;
            MyAuth = _MyAuth;
            Smtp = _service;
        }
        [HttpPost("Login")]
        public async Task<TokenInformacije> Login([FromBody] PrijavaRequest Prijava)
        {
            var korisnik = await  DB.Korisnici.Include("Grad").Where(k => k.Username == Prijava.Username
            && k.Password == Prijava.Password).FirstOrDefaultAsync();
            if (korisnik == null)
            {
                return new TokenInformacije()
                {
                    IsLogiran = false,
                    Token = null,
                    Gost = null,
                    Uposlenik = null,
                    IsHostesa=false,
                    IsGost =false,
                    IsManager=false,
                    Greska=404,
                };
            }
            else
            {
              
                if(korisnik.isGost)
                {
                    var token = TokenGenerator.GenerisiToken();
                    var prijava = new Prijava()
                    {
                        KorisnikKorisnikID=korisnik.KorisnikID,
                        DatumVrijeme = DateTime.Now,
                        Token = token,
                        IsGost= true,
                        IsManager = false,
                        IsHostesa = false,
                    };
                    DB.Prijave.Add(prijava);
                    DB.SaveChanges();
                    return new TokenInformacije()
                    {
                        IsLogiran = true,
                        Gost = korisnik as Gost,
                        Token = token,
                        Uposlenik = null,
                        IsGost = true,
                        IsHostesa = false,
                        IsManager = false,
                        Greska=200
                    };
                }
                else
                {
                    var token = TokenGenerator.GenerisiToken();
                    var prijava = new Prijava()
                    {
                        KorisnikKorisnikID = korisnik.KorisnikID,
                        DatumVrijeme = DateTime.Now,
                        Token = token,
                        IsGost = false,
                        IsManager = korisnik.isManager,
                        IsHostesa = korisnik.isHostesa
                    };
                    DB.Prijave.Add(prijava);
                    DB.SaveChanges();
                    return new TokenInformacije()
                    {
                        IsLogiran = true,
                        Gost = null,
                        Token = token,
                        Uposlenik = korisnik as Uposlenik,
                        IsGost = false,
                        IsHostesa = korisnik.isHostesa,
                        IsManager = korisnik.isManager,
                        Greska=200
                    };
                }
            }
        }
        [HttpPost("Registracija")]
        public async Task<RegistracijaResponse> Registracija([FromBody] RegistracijaRequest Registracija)
        {
            if (string.IsNullOrWhiteSpace(Registracija.Ime) ||
                string.IsNullOrWhiteSpace(Registracija.Prezime) ||
                string.IsNullOrWhiteSpace(Registracija.BrojTelefona) ||
                string.IsNullOrWhiteSpace(Registracija.Email) ||
                string.IsNullOrWhiteSpace(Registracija.Username) ||
                string.IsNullOrWhiteSpace(Registracija.Password) || Registracija.GradID == 0)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Sva polja su obavezna!"
                };
            }
            if (Registracija.Password.Length < 8)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Lozinka mora sadrzavati minimalno 8 karaktera!"
                };
            }
            if (Registracija.DatumRodjenja > DateTime.Now.AddYears(-18))
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Korisnik mora biti stariji od 18 godina!"
                };
            }
            if (!CheckAttributesHelper.IspravanTelefon(Registracija.BrojTelefona))
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Neispravno unesen format broja telefona! Ispravan format: +38761111000"
                };
            }
            if (!CheckAttributesHelper.IspravanEmail(Registracija.Email))
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Neispravno unesen format e-mail naloga! Ispravan format: example@mail.com"
                };
            }
            if ((await PostojiBrojTelefona(Registracija.BrojTelefona)).Postoji)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            if ((await PostojiEmail(Registracija.Email)).Postoji)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if ((await PostojiUsername(Registracija.Username)).Postoji)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            try
            {
                var gost = new Gost()
                {
                    Ime = Registracija.Ime,
                    Prezime = Registracija.Prezime,
                    DatumRodjenja = Registracija.DatumRodjenja,
                    BrojTelefona = Registracija.BrojTelefona,
                    Email = Registracija.Email,
                    Username = Registracija.Username,
                    Password = Registracija.Password,
                    GradID = Registracija.GradID,
                    Aktivan = false,
                    DatumRegistracije = DateTime.Now,
                    BrojRezervacija = 0,
                    Slika="profilne-slike/empty.jpg"
                };
                DB.Gosti.Add(gost);
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                return new RegistracijaResponse()
                {
                    Registrovan = false,
                    Greska = "Neuspjesna obrada registracije!"
                };
            }
            return new RegistracijaResponse()
            {
                Registrovan = true,
                Greska = ""
            };
        }
        [HttpGet("SlanjeVerifikacijeTelefon")]
        public async Task<bool> SlanjeVerifikacijeTelefon()
        {
            var prijava = await MyAuth.isLogiran();
            if (!prijava.JePrijavljen)
            {
                throw new UnauthorizedAccessException();
            }
            if (!prijava.IsGost) throw new UnauthorizedAccessException();
            if (prijava.Prijava!.Korisnik.Aktivan) return false;
            var verifikacija = await DB.KorisnikTelefon.FirstOrDefaultAsync(ke => ke.GostId == prijava.Prijava.KorisnikKorisnikID);
            if (verifikacija == null)
            {
                verifikacija = new KorisnikTelefon
                {
                    GostId = prijava.Prijava.KorisnikKorisnikID,
                    BrojIzdavanja = 1,
                    VrijemeIsteka = DateTime.Now.AddMinutes(2),
                    VrijemePrvogIzdavanja = DateTime.Now,
                    Key = UserKeyGenerator.GenerisiPhoneKey(),
                    Verifikovan = false
                };
                DB.KorisnikTelefon.Add(verifikacija);
                DB.SaveChanges();
                var tekst = $"Verifikacijski kod za potvrdu broja telefona: {verifikacija.Key} .Kod vrijedi 2 minuta.";
                SmsService.posaljiPoruku(prijava.Prijava.Korisnik.BrojTelefona, tekst);
                return true;
            }
            else
            {
                if (verifikacija.Verifikovan) return false;
                if (DateTime.Now > verifikacija.VrijemePrvogIzdavanja.AddHours(24))
                {
                    verifikacija.BrojIzdavanja = 0;
                    DB.SaveChanges();
                }
                if (verifikacija.BrojIzdavanja == 3)
                {
                    return false;
                }
                if (verifikacija.BrojIzdavanja == 0)
                {
                    verifikacija.VrijemePrvogIzdavanja = DateTime.Now;
                }
                verifikacija.BrojIzdavanja = verifikacija.BrojIzdavanja + 1;
                    verifikacija.VrijemeIsteka = DateTime.Now.AddMinutes(2);
                    verifikacija.Key = UserKeyGenerator.GenerisiPhoneKey();
                    DB.SaveChanges();
                var tekst = $"Verifikacijski kod za potvrdu broja telefona: {verifikacija.Key} .Kod vrijedi 2 minuta.";
                SmsService.posaljiPoruku(prijava.Prijava.Korisnik.BrojTelefona, tekst);
                return true;
            }

        }
        [HttpGet("VerifikujTelefon")]
        public async Task<bool> VerifikujTelefon (string key)
        {
            var verifikacija = await DB.KorisnikTelefon.Include(ke => ke.Gost).FirstOrDefaultAsync(ke => ke.Key == key);
            if (verifikacija == null) return false;
            if (verifikacija.Verifikovan) return false;
            if (DateTime.Now > verifikacija.VrijemeIsteka) return false;
            var korisnik = verifikacija.Gost;
            verifikacija.Verifikovan = true;
            DB.SaveChanges();
            var email = await DB.KorisnikEmail.FirstOrDefaultAsync(ke => ke.GostId == korisnik.KorisnikID);
            if (email == null) return true;
            if (email.Verifikovan)
            {
                korisnik.Aktivan = true;
                DB.SaveChanges();
            }
            return true;
        }

        [HttpGet("SlanjeVerifikacijeEmail")]
        public async Task<bool> SlanjeVerifikacijeEmail ()
        {
            var prijava = await MyAuth.isLogiran();
            if (!prijava.JePrijavljen)
            {
                throw new UnauthorizedAccessException();
            }
            if(!prijava.IsGost) throw new UnauthorizedAccessException();
            if (prijava.Prijava!.Korisnik.Aktivan) return false;
            var verifikacija = await DB.KorisnikEmail.FirstOrDefaultAsync(ke => ke.GostId == prijava.Prijava.KorisnikKorisnikID);
            if (verifikacija == null)
            {
                verifikacija = new KorisnikEmail
                {
                    GostId = prijava.Prijava.KorisnikKorisnikID,
                    BrojIzdavanja = 1,
                    VrijemeIsteka = DateTime.Now.AddMinutes(30),
                    VrijemePrvogIzdavanja = DateTime.Now,
                    Key = UserKeyGenerator.GenerisiKey(),
                    Verifikovan = false
                };
                DB.KorisnikEmail.Add(verifikacija);
                DB.SaveChanges();
                var tekst = $"<h3 style='display:block;color:black' >Vaši novi pristupni podaci za ERezervacije:</h3>" +
             $"<a href='{AddressesHelper.WebPageAddress}#/verifikacija-email/{verifikacija.Key}' >Kliknite ovdje za verifikaciju e-mail naloga</a>" +
             $"<p>Verifikacijski link vrijedi 30 minuta.</p>";
                bool success = await Smtp.sendEmail("ERezervacije - Verifikacija e-mail naloga", tekst, prijava.Prijava!.Korisnik.Email);
                return true;
            }
            else
            {
                if (verifikacija.Verifikovan) return false;
                if(DateTime.Now > verifikacija.VrijemePrvogIzdavanja.AddHours(24))
                {
                    verifikacija.BrojIzdavanja = 0;
                    DB.SaveChanges();
                }
                if(verifikacija.BrojIzdavanja ==10)
                {
                    return false;
                }
                if (verifikacija.BrojIzdavanja == 0)
                {
                    verifikacija.VrijemePrvogIzdavanja = DateTime.Now;
                }
                verifikacija.BrojIzdavanja = verifikacija.BrojIzdavanja + 1;
                    verifikacija.VrijemeIsteka = DateTime.Now.AddMinutes(30);
                    verifikacija.Key = UserKeyGenerator.GenerisiKey();
                    DB.SaveChanges();

                var tekst = $"<h3 style='display:block;color:black' >Verifikacija e-mail naloga:</h3>" +
              $"<a href='{AddressesHelper.WebPageAddress}#/verifikacija-email/{verifikacija.Key}' >Kliknite ovdje za verifikaciju e-mail naloga</a>" +
              $"<p>Verifikacijski link vrijedi 30 minuta.</p>";
                bool success = await Smtp.sendEmail("ERezervacije - Verifikacija e-mail naloga", tekst, prijava.Prijava!.Korisnik.Email);
                return true;
            }
           
        }
        [HttpGet("VerifikujEmail")]
        public async Task<bool> VerifikujEmail (string key)
        {
            var verifikacija = await DB.KorisnikEmail.Include(ke=> ke.Gost).FirstOrDefaultAsync(ke => ke.Key == key);
            if (verifikacija == null) return false;
            if (verifikacija.Verifikovan) return false;
            if (DateTime.Now > verifikacija.VrijemeIsteka) return false;
            var korisnik = verifikacija.Gost;
            verifikacija.Verifikovan = true;
            DB.SaveChanges();
            var telefon = await DB.KorisnikTelefon.FirstOrDefaultAsync(ke => ke.GostId == korisnik.KorisnikID);
            if (telefon == null) return true;
            if (telefon.Verifikovan)
            {
                korisnik.Aktivan = true;
                DB.SaveChanges();
            }
            return true;
        }
        [HttpGet("Odjava")]
        public async Task<OdjavaResponse> Odjava()
        {
            var prijava = await MyAuth.isLogiran();
            if (!prijava.JePrijavljen)
            {
                return new OdjavaResponse() { Odjavljen = false };
            }
            var prijava2 = await DB.Prijave.FindAsync(prijava.Prijava!.PrijavaID);
            DB.Prijave.Remove(prijava2);
            DB.SaveChanges();
            return new OdjavaResponse()
            {
                Odjavljen = true
            };
        }
        [HttpGet("PostojiUsername")]
        public async Task<UsernameResponse> PostojiUsername([FromQuery] string username)
        {
            var prijava = await MyAuth.isLogiran();
            var prijavljen = prijava.JePrijavljen;
            var kime = "";
            if (prijavljen)
            {
               kime = prijava.Prijava!.Korisnik.Username;
            }
            if ((await DB.Korisnici.Where(k => k.Username == username && kime!=username).CountAsync()) == 0)
                return new UsernameResponse()
                {
                    Postoji = false
                };
            return new UsernameResponse()
            {
                Postoji = true
            };
        }
        [HttpGet("PostojiEmail")]
        public async Task<EmailResponse> PostojiEmail([FromQuery] string email)
        {
            var prijava = await MyAuth.isLogiran();
            var prijavljen = prijava.JePrijavljen;
            var kemail = "";
            if(prijavljen)
            {
                kemail = prijava.Prijava!.Korisnik.Email;
            }
            if ((await DB.Korisnici.Where(k => k.Email == email && kemail!=email).CountAsync()) == 0
                && (await DB.BlokiraniKorisnici.Where(bk=> bk.Email == email && kemail!=email).CountAsync())==0)
                return new EmailResponse()
                {
                    Postoji = false
                };
            return new EmailResponse()
            {
                Postoji = true
            };
        }
        [HttpGet("PostojiBrojTelefona")]
        public async Task<TelefonResponse> PostojiBrojTelefona([FromQuery] string telefon)
        {
            var prijava = await MyAuth.isLogiran();
            var prijavljen = prijava.JePrijavljen;
            var ktelefon = "";
            if(prijavljen)
            {
                ktelefon = prijava.Prijava!.Korisnik.BrojTelefona;
            }
            if ((await DB.Korisnici.Where(k => k.BrojTelefona == telefon && ktelefon!=telefon).CountAsync()) == 0 &&
                (await DB.BlokiraniKorisnici.Where(bk=> bk.BrojTelefona == telefon && ktelefon!=telefon).CountAsync())==0)
                return new TelefonResponse()
                {
                    Postoji = false
                };
            return new TelefonResponse()
            {
                Postoji = true
            };
        }

    }
}
