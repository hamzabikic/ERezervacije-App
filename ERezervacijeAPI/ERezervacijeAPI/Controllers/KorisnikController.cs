using ERezervacijeAPI.AutentifikacijaKlase;
using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.KorisnikKlase;
using ERezervacijeAPI.ManagerKlase;
using ERezervacijeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    [Auth]
    public class KorisnikController
    {
        public readonly DBConnection db;
        private readonly MyAuthService auth;
        private readonly CheckAttributesHelper checkAttributes;
        public KorisnikController (DBConnection dbconnection, MyAuthService _auth, CheckAttributesHelper _check)
        {
            db = dbconnection;
            auth = _auth;
            checkAttributes = _check;
        }

        [HttpGet("GetGost")]
        public async Task<GostResponse> GetKorisnik()
        {
            var prijava = await auth.isLogiran();
            var korisnik = prijava.Prijava!.Korisnik as Gost;
            if (korisnik == null) throw new UnauthorizedAccessException();
            return new GostResponse
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                DatumRodjenja = korisnik.DatumRodjenja,
                Username = korisnik.Username,
                Email = korisnik.Email,
                BrojTelefona = korisnik.BrojTelefona,
                GradID = korisnik.GradID,
                Grad = korisnik.Grad,
                BrojRezervacija = korisnik.BrojRezervacija,
                DatumRegistracije = korisnik.DatumRegistracije.ToShortDateString(),
                Aktivan = korisnik.Aktivan,
                VerifikovanEmail = (await db.KorisnikEmail.FirstOrDefaultAsync(ke=> ke.GostId == korisnik.KorisnikID))?.Verifikovan ?? false,
                VerifikovanTelefon = (await db.KorisnikTelefon.FirstOrDefaultAsync(ke => ke.GostId == korisnik.KorisnikID))?.Verifikovan ?? false
            };
        }
        [HttpGet("GetPopustiHostesa")]
        public async Task<List<PopustResponse>> GetPopustiHostesa(string? imePrezime ="")
        {
            var prijava = await auth.isLogiran();
            var korisnik = prijava.Prijava!.Korisnik as Hostesa;
            if (korisnik == null) throw new UnauthorizedAccessException();
            return await db.GostiPopusti.Include(p => p.Popust).Include(p => p.Gost).Where(p => p.Iskoristen == false
            && (string.IsNullOrWhiteSpace(imePrezime) ||(p.Gost.Ime+" " + p.Gost.Prezime).ToLower().Contains(imePrezime.ToLower()))).
                Select(p => new PopustResponse
                {
                    ImePrezime = p.Gost.Ime + " " + p.Gost.Prezime,
                    PopustId = p.GostPopustID,
                    Iznos = p.Popust.Procenat,
                    Razlog = p.Popust.Razlog
                }).Take(20).ToListAsync();
        }
        [HttpGet("IskoristiPopust")]
        public async Task<bool> IskoristiPopust (int id)
        {
            var prijava = await auth.isLogiran();
            var korisnik = prijava.Prijava!.Korisnik as Hostesa;
            if (korisnik == null) throw new UnauthorizedAccessException();
            var popust = await db.GostiPopusti.FindAsync(id);
            if (popust == null) return false;
            popust.Iskoristen = true;
            db.SaveChanges();
            return true;
        }
        [HttpPost("PromjenaLozinke")]
        public async Task<LozinkaResponse> PromjenaLozinke (LozinkaRequest nova)
        {
            if(string.IsNullOrWhiteSpace(nova.StaraLozinka) || string.IsNullOrWhiteSpace(nova.NovaLozinka))
            {
                return new LozinkaResponse
                {
                    Promijenjena = false,
                    Greska = "Obavezna polja!"
                };
            }
            if(nova.NovaLozinka.Length<8 || nova.StaraLozinka.Length <8)
            {
                return new LozinkaResponse
                {
                    Promijenjena = false,
                    Greska = "Vasa lozinka mora sadrzavati minimalno 8 karaktera!"
                };
            }
            var prijava = await auth.isLogiran();
            if (prijava.Prijava!.Korisnik.Password != nova.StaraLozinka) return new LozinkaResponse
            {
                Promijenjena = false,
                Greska ="Trenutna lozinka neispravno unesena!"
            };
            if (nova.StaraLozinka == nova.NovaLozinka) return new LozinkaResponse
            {
                Promijenjena = false,
                Greska = "Unesena lozinka se trenutno koristi!"
            };
            var korisnik = await db.Korisnici.FindAsync(prijava.Prijava.KorisnikKorisnikID);
            korisnik!.Password = nova.NovaLozinka;
            db.SaveChanges();
            return new LozinkaResponse
            {
                Promijenjena = true,
                Greska = ""
            };
        }
        [HttpGet("BrisanjeNaloga")]
        public async Task<BrisanjeNalogaResponse> BrisanjeNaloga()
        {
            var prijava = await auth.isLogiran();
            if(!prijava.IsGost)
            {
                throw new UnauthorizedAccessException();
            }
            if((await db.Rezervacije.Where(r=> r.GostID == prijava.Prijava.KorisnikKorisnikID && DateTime.Now < r.DatumVrijeme.AddHours(r.Trajanje) && !r.Preuzeta && !r.Otkazana && !r.Ponistena).CountAsync())>0)
            {
                return new BrisanjeNalogaResponse
                {
                    Obrisan = false,
                    Greska = "Trenutno nije moguće obrisati nalog, jer imate aktivnu rezervaciju!"
                };
            }
            var brojRezervacija = await db.Rezervacije.Where(r =>r.GostID == prijava.Prijava.KorisnikKorisnikID && DateTime.Now > r.DatumVrijeme.AddHours(r.Trajanje) && !r.Preuzeta && !r.Otkazana && !r.Ponistena && r.Odobrena).CountAsync();
            var gost = await db.Gosti.FindAsync(prijava.Prijava.KorisnikKorisnikID);
            if(brojRezervacija!=0)
            {
                var blokiran = new BlokiraniKorisnik
                {
                    Email = gost.Email,
                    BrojTelefona = gost.BrojTelefona
                };
                db.BlokiraniKorisnici.Add(blokiran);
                db.SaveChanges();
            }
            var rezervacije = await db.Rezervacije.Where(r => r.GostID == gost.KorisnikID).ToListAsync();
            if (rezervacije.Count() > 0)
            {
                db.Rezervacije.RemoveRange(rezervacije);
                db.SaveChanges();
            }
            db.Gosti.Remove(gost);
            db.SaveChanges();
            return new BrisanjeNalogaResponse
            {
                Obrisan = true,
                Greska = ""
            };
        }

        [HttpGet("GetUposlenik")]
        public async Task<UposlenikResponse> GetUposlenik()
        {
            var prijava = await auth.isLogiran();
            var korisnik = prijava.Prijava!.Korisnik as Uposlenik;
            if (korisnik == null) throw new UnauthorizedAccessException();
            return new UposlenikResponse
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                DatumRodjenja = korisnik.DatumRodjenja,
                Username = korisnik.Username,
                Email = korisnik.Email,
                BrojTelefona = korisnik.BrojTelefona,
                GradID = korisnik.GradID,
                Grad = korisnik.Grad,
                DatumZaposlenja = korisnik.DatumZaposlenja,
                StrucnaSprema= korisnik.StrucnaSprema
            };
        }
        
        [HttpPost("EditGost")]
        public async Task<KorisnikEditResponse> EditGost (KorisnikEditPost edit)
        {
            if (string.IsNullOrWhiteSpace(edit.Ime) ||
                string.IsNullOrWhiteSpace(edit.Prezime) ||
                string.IsNullOrWhiteSpace(edit.BrojTelefona) ||
                string.IsNullOrWhiteSpace(edit.Email) ||
                string.IsNullOrWhiteSpace(edit.Username) || edit.GradID == 0)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Sva polja su obavezna!"
                };
            }
            if(edit.DatumRodjenja > DateTime.Now.AddYears(-18))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Korisnik mora biti stariji od 18 godina!"
                };
            }

            if (!CheckAttributesHelper.IspravanTelefon(edit.BrojTelefona))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neispravno unesen format broja telefona! Ispravan format: +38761111000"
                };
            }
            if (!CheckAttributesHelper.IspravanEmail(edit.Email))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neispravno unesen format e-mail naloga! Ispravan format: example@mail.com"
                };
            }
            var prijava = await auth.isLogiran();
            var korisnik = await db.Korisnici.FindAsync(prijava.Prijava!.Korisnik.KorisnikID);
            if (!korisnik!.isGost) throw new UnauthorizedAccessException();
            if((await db.Rezervacije.Where(r=> r.GostID== korisnik.KorisnikID && !r.Otkazana && !r.Ponistena &&
            r.Odobrena && !r.Preuzeta && DateTime.Now > r.DatumVrijeme.AddHours(r.Trajanje)).CountAsync())!=0)
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Vaš korisnički račun je blokiran zbog nepreuzimanja rezervacije!"
                };
            bool postoji = (await db.Rezervacije.Where(r => r.GostID == korisnik.KorisnikID &&
           !r.Ponistena && !r.Otkazana && !r.Preuzeta && DateTime.Now < r.DatumVrijeme.AddHours(r.Trajanje)).CountAsync()) > 0 ? true : false;
            if(postoji)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Trenutno niste u mogucnosti editovati podatke, jer imate aktivnu rezervaciju!"
                };
            }
            if (await checkAttributes.PostojiUsername(edit.Username, korisnik.Username)) {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            if(await checkAttributes.PostojiEmail(edit.Email, korisnik.Email))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if(await checkAttributes.PostojiBrojTelefona(edit.BrojTelefona, korisnik.BrojTelefona))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            bool aktivan = korisnik.Aktivan;
            if(edit.Email != korisnik.Email)
            {
                var email_ver = await db.KorisnikEmail.FirstOrDefaultAsync(ke => ke.GostId == korisnik.KorisnikID);
                if (email_ver != null)
                {
                    email_ver.Verifikovan = false;
                    email_ver.VrijemeIsteka = DateTime.Now;
                    db.SaveChanges();
                }
                aktivan = false;
            }
            if(edit.BrojTelefona != korisnik.BrojTelefona)
            {
                var telefon_ver = await db.KorisnikTelefon.FirstOrDefaultAsync(ke => ke.GostId == korisnik.KorisnikID);
                if (telefon_ver != null)
                {
                    telefon_ver.Verifikovan = false;
                    telefon_ver.VrijemeIsteka = DateTime.Now;
                    db.SaveChanges();
                }
                aktivan = false;
            }
            try
            {
                korisnik.Ime = edit.Ime;
                korisnik.Prezime = edit.Prezime;
                korisnik.DatumRodjenja = edit.DatumRodjenja;
                korisnik.Email = edit.Email;
                korisnik.BrojTelefona = edit.BrojTelefona;
                korisnik.Username = edit.Username;
                korisnik.GradID = edit.GradID;
                korisnik.Aktivan = aktivan;
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neuspjesno editovanje gosta!"
                };
            }
            return new KorisnikEditResponse
            {
                Editovan = true,
                Greska = ""
            };
        }       
        [HttpGet("GetPopustiGost")]
        public async Task<List<Popust>> GetPopustiGost ()
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsGost) throw new UnauthorizedAccessException();
            List<Popust> popusti = await db.GostiPopusti.Include(gp => gp.Gost).Include(gp => gp.Popust).Where(
                gp => gp.GostID == prijava.Prijava!.KorisnikKorisnikID && gp.Iskoristen==false).Select(
                gp => gp.Popust).ToListAsync();
            return popusti;
        }


    }
}
