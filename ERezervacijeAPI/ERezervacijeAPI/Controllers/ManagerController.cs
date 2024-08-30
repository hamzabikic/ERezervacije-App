using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.ManagerKlase;
using ERezervacijeAPI.RezervacijaKlase;
using ERezervacijeAPI.Services;
using ERezervacijeAPI.StoloviKlase;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.Data.Common;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    [Manager]
    public class ManagerController
    {
        private readonly DBConnection db;
        private readonly MyAuthService auth;
        private readonly SmtpService smtp;
        private readonly CheckAttributesHelper checkAttributes;
        public ManagerController(DBConnection _db, MyAuthService _auth, SmtpService _smtp, CheckAttributesHelper _check)
        {
            db = _db;
            auth = _auth;
            smtp = _smtp;
            checkAttributes = _check;
        }
        [HttpGet("GetGostiManager")]
        public async Task<List<GostManagerResponse>> GetGostiManager (string? imePrezime ="")
        {
            return await db.Gosti.Include(g => g.Grad).Where(g => imePrezime == "" ||
            (g.Ime + " " + g.Prezime).ToLower().Contains(imePrezime.ToLower())).Select(
                g => new GostManagerResponse
                {
                    Ime = g.Ime,
                    Prezime = g.Prezime,
                    DatumRegistracije = g.DatumRegistracije,
                    BrojRezervacija = g.BrojRezervacija,
                    DatumRodjenja= g.DatumRodjenja,
                    GradID = g.GradID,
                    Grad = g.Grad,
                    BrojTelefona= g.BrojTelefona,
                    KorisnikID = g.KorisnikID,
                    Email= g.Email,
                    Username= g.Username
                }).OrderByDescending(g=> g.KorisnikID).Take(100).ToListAsync();
        }
        [HttpPost("EditGostManager")]
        public async Task<KorisnikEditResponse> EditGostManager(KorisnikManagerEdit edit)
        {
            var gost = await db.Gosti.FindAsync(edit.KorisnikId);
            if (gost == null) return new KorisnikEditResponse
            {
                Editovan = false,
                Greska = "Gost nije pronadjen u sistemu!"
            };
            if(string.IsNullOrWhiteSpace(edit.Ime) || string.IsNullOrWhiteSpace(edit.Prezime) || string.IsNullOrWhiteSpace(edit.Username)
                || string.IsNullOrWhiteSpace(edit.BrojTelefona)||  string.IsNullOrWhiteSpace(edit.Email) ||
                    edit.GradId<1)
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
            if (await checkAttributes.PostojiUsername(edit.Username,gost.Username))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiEmail(edit.Email, gost.Email))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiBrojTelefona(edit.BrojTelefona, gost.BrojTelefona))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            try
            {
                gost.Ime = edit.Ime;
                gost.Prezime = edit.Prezime;
                gost.Email = edit.Email;
                gost.Username = edit.Username;
                gost.BrojTelefona = edit.BrojTelefona;
                gost.GradID = edit.GradId;
                gost.DatumRodjenja = edit.DatumRodjenja;
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
        [HttpGet("IzbrisiKorisnikManager")]
        public async Task<bool> IzbrisiKorisnikManager(int id)
        {
            var korisnik = await db.Korisnici.FindAsync(id);
            if (korisnik == null) return false;
            if(korisnik.isManager) return false;
            db.Korisnici.Remove(korisnik);
            db.SaveChanges();
            return true;
        }
        [HttpPost("EditManager")]
        public async Task<KorisnikEditResponse> EditManager(HostesaManagerEdit edit)
        {
            var prijava = await auth.isLogiran();
            var manager = await db.Manageri.FindAsync(prijava.Prijava!.KorisnikKorisnikID);
            if (string.IsNullOrWhiteSpace(edit.Ime) || string.IsNullOrWhiteSpace(edit.Prezime) || string.IsNullOrWhiteSpace(edit.Username)
               || string.IsNullOrWhiteSpace(edit.BrojTelefona) || string.IsNullOrWhiteSpace(edit.Email) ||
               string.IsNullOrWhiteSpace(edit.StrucnaSprema) ||
                   edit.GradId < 1)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Sva polja su obavezna!"
                };
            }
            if (edit.DatumRodjenja > DateTime.Now.AddYears(-18))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Korisnik mora biti stariji od 18 godina!"
                };
            }

            if (DateTime.Now < edit.DatumZaposlenja)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neispravno unesen datum zaposlenja!"
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
            if (await checkAttributes.PostojiUsername(edit.Username, manager.Username))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiEmail(edit.Email, manager.Email))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiBrojTelefona(edit.BrojTelefona,manager.BrojTelefona))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            try
            {
                manager.Ime = edit.Ime;
                manager.Prezime = edit.Prezime;
                manager.Email = edit.Email;
                manager.Username = edit.Username;
                manager.BrojTelefona = edit.BrojTelefona;
                manager.GradID = edit.GradId;
                manager.DatumRodjenja = edit.DatumRodjenja;
                manager.DatumZaposlenja = edit.DatumZaposlenja;
                manager.StrucnaSprema = edit.StrucnaSprema;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neuspjesno editovanje hostese!"
                };
            }
            return new KorisnikEditResponse
            {
                Editovan = true,
                Greska = ""
            };


        }
        [HttpGet("GetHosteseManager")]
        public async Task<List<HostesaManagerResponse>> GetHosteseManager (string? imePrezime="")
        {
            return await db.Hostese.Include(h => h.Grad).Where(h => imePrezime == "" || (h.Ime + " " + h.Prezime).ToLower().Contains(
                imePrezime!.ToLower())).Select(
                h => new HostesaManagerResponse
                {
                    Ime = h.Ime,
                    Prezime = h.Prezime,
                    DatumRodjenja = h.DatumRodjenja,
                    Grad = h.Grad,
                    GradID = h.GradID,
                    BrojTelefona = h.BrojTelefona,
                    Email = h.Email,
                    DatumZaposlenja = h.DatumZaposlenja,
                    KorisnikID = h.KorisnikID,
                    StrucnaSprema = h.StrucnaSprema,
                    Username = h.Username,
                }).OrderByDescending(h => h.KorisnikID).Take(100).ToListAsync();
        }
        [HttpPost("EditHostesaManager")]
        public async Task<KorisnikEditResponse> EditHostesaManager (HostesaManagerEdit edit)
        {
            var hostesa = await db.Hostese.FindAsync(edit.KorisnikId);
            if (hostesa == null) return new KorisnikEditResponse
            {
                Editovan = false,
                Greska = "Hostesa nije pronadjena u sistemu!"
            };
            if (string.IsNullOrWhiteSpace(edit.Ime) || string.IsNullOrWhiteSpace(edit.Prezime) || string.IsNullOrWhiteSpace(edit.Username)
                || string.IsNullOrWhiteSpace(edit.BrojTelefona) || string.IsNullOrWhiteSpace(edit.Email) ||
                string.IsNullOrWhiteSpace(edit.StrucnaSprema) ||
                    edit.GradId < 1)
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

            if(DateTime.Now < edit.DatumZaposlenja)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neispravno unesen datum zaposlenja!"
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
            if (await checkAttributes.PostojiUsername(edit.Username, hostesa.Username))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiEmail(edit.Email, hostesa.Email))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiBrojTelefona(edit.BrojTelefona, hostesa.BrojTelefona))
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            try
            {
                hostesa.Ime = edit.Ime;
                hostesa.Prezime = edit.Prezime;
                hostesa.Email = edit.Email;
                hostesa.Username = edit.Username;
                hostesa.BrojTelefona = edit.BrojTelefona;
                hostesa.GradID = edit.GradId;
                hostesa.DatumRodjenja = edit.DatumRodjenja;
                hostesa.DatumZaposlenja = edit.DatumZaposlenja;
                hostesa.StrucnaSprema = edit.StrucnaSprema;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new KorisnikEditResponse
                {
                    Editovan = false,
                    Greska = "Neuspjesno editovanje hostese!"
                };
            }
            return new KorisnikEditResponse
            {
                Editovan =true,
                Greska = ""
            };

        }

        [HttpPost("AddHostesaManager")]
        public async Task<AddHostesaResponse> AddHostesaManager(HostesaAddManager add)
        {
            if (string.IsNullOrWhiteSpace(add.Ime) || string.IsNullOrWhiteSpace(add.Prezime) || string.IsNullOrWhiteSpace(add.Username)
                 || string.IsNullOrWhiteSpace(add.BrojTelefona) || string.IsNullOrWhiteSpace(add.Email) ||
                 string.IsNullOrWhiteSpace(add.StrucnaSprema) ||
                     add.GradID < 1)
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Sva polja su obavezna!"
                };
            }
            if (add.DatumRodjenja > DateTime.Now.AddYears(-18))
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Korisnik mora biti stariji od 18 godina!"
                };
            }

            if (DateTime.Now < add.DatumZaposlenja)
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Neispravno unesen datum zaposlenja!"
                };
            }
            if (!CheckAttributesHelper.IspravanTelefon(add.BrojTelefona))
            {
                return new AddHostesaResponse
                {
                    HostesaId = 0,
                    Greska = "Neispravno unesen format broja telefona! Ispravan format: +38761111000"
                };
            }
            if (!CheckAttributesHelper.IspravanEmail(add.Email))
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Neispravno unesen format e-mail naloga! Ispravan format: example@mail.com"
                };
            }
            if (await checkAttributes.PostojiUsername(add.Username, ""))
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Uneseni username vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiEmail(add.Email, ""))
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Uneseni email vec postoji u sistemu!"
                };
            }
            if (await checkAttributes.PostojiBrojTelefona(add.BrojTelefona,""))
            {
                return new AddHostesaResponse
                {
                    HostesaId=0,
                    Greska = "Uneseni broj telefona vec postoji u sistemu!"
                };
            }
            var nova = new Hostesa
            {
                Ime = add.Ime,
                Prezime = add.Prezime,
                DatumRodjenja = add.DatumRodjenja,
                Username = add.Username,
                BrojTelefona = add.BrojTelefona,
                Email = add.Email,
                Aktivan = true,
                DatumZaposlenja = add.DatumZaposlenja,
                GradID = add.GradID,
                Password = "testtest",
                StrucnaSprema = add.StrucnaSprema,
                Slika="profilne-slike/empty.jpg"
            };
            try
            {
                db.Hostese.Add(nova);
                db.SaveChanges();
            }catch(Exception ex)
            {
                return new AddHostesaResponse
                {
                    HostesaId= 0,
                    Greska = "Neuspjesno dodavanje hostese!"
                };
            }
            return new AddHostesaResponse
            {
                HostesaId = nova.KorisnikID,
                Greska = ""
            };
        }
        [HttpGet("GenerisiNovuLozinku")]
        public async Task<bool> GenerisiNovuLozinku(int korisnikId)
        {
            var korisnik = await db.Korisnici.FindAsync(korisnikId);
            if (korisnik == null) return false;
            if (korisnik.isManager) return false;
            var lozinka = PasswordGenerator.generisiLozinku();
            var tekst = $"<h3 style='display:block;color:black' >Vaši novi pristupni podaci za ERezervacije:</h3>" +
                $"<p style='color:black' >Username: <b>{korisnik.Username}</b></p>" +
                $"<p style='color:black' >Password: <b>{lozinka}</b></p>";
            bool success = await smtp.sendEmail("Novi pristupni podaci", tekst, korisnik.Email);
            if (success)
            {
                try
                {
                    korisnik.Password = lozinka;
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        [HttpGet("GetRezervacijeManager")]
        public async Task<List<RezervacijaHostesaResponse>> GetRezervacijeManager(string? id="")
        {
            return await db.Rezervacije.Where(r => id=="" || r.RezervacijaID.ToString().Contains(id)).
                Select(r => new RezervacijaHostesaResponse
                {
                    DatumRezervacije = r.DatumVrijeme.ToShortDateString(),
                    Odobrena = r.Odobrena,
                    Preuzeta = r.Preuzeta,
                    RezervacijaID = r.RezervacijaID,
                    VrijemeRezervacije = r.DatumVrijeme.ToShortTimeString() + " - " + r.DatumVrijeme.AddHours(r.Trajanje).ToShortTimeString(),
                    Stolovi = db.RezervacijeStolovi.Include(rs => rs.Stol).Where(rs => rs.RezervacijaID == r.RezervacijaID).Select(
                        rs => rs.Stol.BrojStola).ToList(),
                    Recenzirano = db.Recenzije.Where(rec => rec.RezervacijaId == r.RezervacijaID).Count() == 0 ? false : true,
                    BrojTelefona = r.Gost == null ? r.BrojTelefona! : r.Gost!.BrojTelefona,
                    ImePrezime = r.Gost == null ? r.ImePrezime! : r.Gost!.Ime +" " + r.Gost!.Prezime,
                    Ponistena = r.Ponistena,
                    Otkazana = r.Otkazana
                }).OrderByDescending(r => r.RezervacijaID).Take(100).ToListAsync();
        }
        [HttpGet("ObrisiRezervacijuManager")]
        public async Task<bool> ObrisiRezervacijuManager(int id)
        {
            var rezervacija = await  db.Rezervacije.FindAsync(id);
            if (rezervacija == null) return false;
            db.Rezervacije.Remove(rezervacija);
            db.SaveChanges();
            return true;
        }
        [HttpPost("ZabranaStolova")]

        public async Task<RezervacijaResponse> ZabranaStola (ZabranaStolaPost zabrana)
        {
            if (zabrana.PocetakVrijeme >= zabrana.KrajVrijeme) return new RezervacijaResponse
            {
                RezervacijaID = 0,
                Greska = "Nepravilan unos datuma!"
            };
            if (DateTime.Now >= zabrana.PocetakVrijeme) return new RezervacijaResponse
            {
                RezervacijaID = 0,
                Greska = "Nepravilan unos datuma!"
            };
            if (zabrana.PocetakVrijeme.Minute != 0 || zabrana.PocetakVrijeme.Second != 0 ||
                zabrana.PocetakVrijeme.Millisecond != 0 ||
                zabrana.KrajVrijeme.Minute!=0 || zabrana.KrajVrijeme.Second!=0 ||
                zabrana.KrajVrijeme.Millisecond!=0)  return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Nepravilan unos satnice!"
                };
            if(zabrana.Stolovi.Count() ==0) return new RezervacijaResponse
            {
                RezervacijaID = 0,
                Greska = "Niste odabrali stolove!"
            };
            var slobodniStolovi = (await GetSlobodniStoloviZabrana(new StoloviRequestZabrana
            {
                DatumPocetak = zabrana.PocetakVrijeme,
                DatumKraj = zabrana.KrajVrijeme
            })).Stolovi.Select(s => s.StolID).ToList();

            foreach(int s in zabrana.Stolovi)
            {
                if(!slobodniStolovi.Contains(s))
                {
                    return new RezervacijaResponse
                    {
                        RezervacijaID = 0,
                        Greska = "Odabrani stolovi su zauzeti u podeseno vrijeme zabrane!"
                    };
                }
            }
            var obj = new Zabrana
            {
                PocetakVrijeme = zabrana.PocetakVrijeme,
                KrajVrijeme = zabrana.KrajVrijeme
            };
            try
            {
                db.Zabrane.Add(obj);
                db.SaveChanges();
                foreach (int stol in zabrana.Stolovi)
                {
                    var zabranaStol = new ZabranaStol
                    {
                        StolId = stol,
                        ZabranaId = obj.ZabranaId
                    };
                    db.ZabranaStol.Add(zabranaStol);
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Neuspjesno dodavanje!"
                };
            }
            if (db.Zabrane.Count() > 100)
            {
                List<Zabrana> zabrane = await db.Zabrane.OrderBy(z => z.PocetakVrijeme).ToListAsync();
                db.Zabrane.Remove(zabrane[0]);
                db.SaveChanges();
            }
            return new RezervacijaResponse
            {
                RezervacijaID = obj.ZabranaId,
                Greska = ""
            };
        }
        [HttpGet("GetZabrane")]
        public async Task<List<ZabranaResponse>> GetZabrane ()
        {
            return await db.Zabrane.OrderByDescending(z => z.PocetakVrijeme).Take(100).Select(
                z => new ZabranaResponse
                {
                    ZabranaId= z.ZabranaId,
                    PocetakDatum = z.PocetakVrijeme.ToShortDateString(),
                    KrajDatum = z.KrajVrijeme.ToShortDateString(),
                    VrijemeOd = z.PocetakVrijeme.ToShortTimeString(),
                    VrijemeDo = z.KrajVrijeme.ToShortTimeString(),
                    Stolovi= db.ZabranaStol.Include(zs=> zs.Stol).Where(zs=> zs.ZabranaId == z.ZabranaId).
                    Select(zs=> zs.Stol).ToList()
                }
                )
            .ToListAsync();
        }
        [HttpGet("IzbrisiZabranu")]
        public async Task<bool> IzbrisiZabranu(int id)
        {
            var zabrana = await db.Zabrane.FindAsync(id);
            if (zabrana == null) return false;
            db.Zabrane.Remove(zabrana);
            db.SaveChanges();
            return true;
        }
        [HttpPost("GetSlobodniStoloviZabrana")]
        public async Task<StoloviResponse> GetSlobodniStoloviZabrana
            ([FromBody] StoloviRequestZabrana request)
        {
            if (request.DatumPocetak <= DateTime.Now || request.DatumPocetak >= request.DatumKraj)
            {
                return new StoloviResponse()
                {
                    Stolovi = new List<Stol> { }
                };
            }
            List<Stol> stolovi = await db.Stolovi.ToListAsync();
            List<Stol> zauzeti = db.RezervacijeStolovi
                    .Include(rs=> rs.Stol).Include(rs=> rs.Rezervacija).Where(
                    rs=> !rs.Rezervacija.Ponistena && !rs.Rezervacija.Otkazana &&  ((rs.Rezervacija.DatumVrijeme >= request.DatumPocetak &&
                    rs.Rezervacija.DatumVrijeme < request.DatumKraj) || (request.DatumPocetak >= rs.Rezervacija.DatumVrijeme &&
                    request.DatumPocetak < rs.Rezervacija.DatumVrijeme.AddHours(rs.Rezervacija.Trajanje)))
                    ).
                   Select(
                      rs => rs.Stol
                    ).ToList();
            List<Stol> zabranjeni = db.ZabranaStol.Include(zs => zs.Stol).Where(
                zs => (zs.Zabrana.PocetakVrijeme >= request.DatumPocetak && 
                zs.Zabrana.PocetakVrijeme < request.DatumKraj) 
                || (request.DatumPocetak >= zs.Zabrana.PocetakVrijeme &&
               request.DatumPocetak< zs.Zabrana.KrajVrijeme)).Select(zs => zs.Stol).ToList();
            List<Stol> slobodniStolovi = new List<Stol>();
            foreach (Stol stol in stolovi)
            {
                if (!zauzeti.Contains(stol) && !zabranjeni.Contains(stol))
                {
                    slobodniStolovi.Add(stol);
                }
            }
            return new StoloviResponse()
            {
                Stolovi = slobodniStolovi
            };
        }
    }
}
