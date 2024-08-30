using ERezervacijeAPI.Data;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.RecenzijeKlase;
using ERezervacijeAPI.RezervacijaKlase;
using ERezervacijeAPI.Services;
using ERezervacijeAPI.StoloviKlase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    [Auth]
    public class RezervacijaController
    {
        private readonly DBConnection db;
        private readonly MyAuthService auth;
        private readonly SmtpService smtp;
        public RezervacijaController(DBConnection dbconnection, MyAuthService _auth, SmtpService _smtp)
        {
            db = dbconnection;
            auth = _auth;
            smtp = _smtp;
        }
        [HttpPost("kreirajRezervaciju")]
        public async Task<RezervacijaResponse> createReservation
            ([FromBody] RezervacijaPost rezervacija)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsGost) throw new UnauthorizedAccessException();
            if(prijava.Prijava!.KorisnikKorisnikID != rezervacija.GostID) throw new UnauthorizedAccessException();
            if (!prijava.Prijava!.Korisnik.Aktivan)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Trenutno niste u mogucnosti kreirati rezervaciju, jer niste verifikovali vaš e-mail nalog i broj telefona!"
                };
            }
            var brojTrenutnihRezervacija = (await db.Rezervacije.Where(r => r.GostID == prijava.Prijava!.KorisnikKorisnikID && !r.Ponistena
            && !r.Otkazana && !r.Preuzeta && DateTime.Now < r.DatumVrijeme.AddHours(r.Trajanje)).CountAsync());
            if(brojTrenutnihRezervacija !=0)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Trenutno niste u mogucnosti kreirati novu rezervaciju, jer imate aktivnu rezervaciju!"
                };
            }
            var brojRezervacija = (await db.Rezervacije.Where(r => r.GostID == prijava.Prijava!.KorisnikKorisnikID && !r.Ponistena
            && !r.Otkazana && !r.Preuzeta && r.Odobrena).CountAsync());
            if (brojRezervacija>= 1)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Trenutno niste u mogucnosti kreirati rezervaciju, jer imate rezervaciju koja nije preuzeta!"
                };
            }
            
            if(rezervacija.Stolovi.Count() >5)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Maksimalan broj stolova je 5!"
                };
            }
            if(rezervacija.DatumVrijeme > DateTime.Now.AddDays(5))
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce napraviti do 5 dana prije!"
                };
            }
            if(DateTime.Now > rezervacija.DatumVrijeme.AddHours(-6))
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce napraviti maksimalno 6 sati pred pocetak!"
                };
            }
            if (rezervacija.DatumVrijeme.Minute != 0 || rezervacija.DatumVrijeme.Second !=0 ||
                rezervacija.DatumVrijeme.Millisecond!=0)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Neispravan unos satnice rezervacije!"
                };
            }
            if (rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje).Hour > 23 ||
                rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje).Hour < 10)
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce kreirati izmedju 9 i 23 sata!"
                };
            if(rezervacija.DatumVrijeme.Hour > 22 || rezervacija.DatumVrijeme.Hour <9)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce kreirati izmedju 9 i 23 sata!"
                };
            }
            if(rezervacija.Trajanje >4)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervacija moze trajati do 4 sata!"
                };
            }
            if(rezervacija.Trajanje<1)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Minimalno vrijeme trajanja rezervacije je 1 sat!"
                };
            }
            List<int> slobodniStolovi = (await GetSlobodniStolovi(new StoloviRequest
            {
                DatumVrijeme = rezervacija.DatumVrijeme,
                Trajanje = rezervacija.Trajanje
            })).Stolovi.Select(s=> s.StolID).ToList();
            foreach(int stol in rezervacija.Stolovi)
            {
                if(!slobodniStolovi.Contains(stol))
                {
                    return new RezervacijaResponse
                    {
                        RezervacijaID = 0,
                        Greska = "Odabrani stolovi vise nisu slobodni!"
                    };
                }
            }
            Rezervacija reservation = new Rezervacija()
            {
                DatumVrijeme = rezervacija.DatumVrijeme,
                Trajanje = rezervacija.Trajanje,
                GostID = rezervacija.GostID,
                PosebneZelje = rezervacija.PosebneZelje,
                Komentar ="",
                Odobrena=false,
                Ponistena=false,
                Otkazana=false,
                Preuzeta=false
            };
            db.Rezervacije.Add(reservation);
            db.SaveChanges();
            List<int> stolovi = rezervacija.Stolovi;
            foreach(int brojstola in stolovi)
            {
                RezervacijaStol rezervacijastol = new RezervacijaStol()
                {
                   RezervacijaID = reservation.RezervacijaID,
                    StolID = brojstola
                };
                db.RezervacijeStolovi.Add(rezervacijastol);
                db.SaveChanges();
            }
            return new RezervacijaResponse()
            {
                RezervacijaID = reservation.RezervacijaID,
                Greska=""
            };
        }
        [HttpGet("GetRezervacijeGost")]
        public async Task<List<RezervacijaGostResponse>> GetRezervacijeGost()
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsGost) throw new UnauthorizedAccessException();
            return await db.Rezervacije.Include(r => r.Gost.Grad).Where(r => r.GostID == prijava.Prijava.KorisnikKorisnikID).
                Select(r => new RezervacijaGostResponse
                {
                    DatumRezervacije = r.DatumVrijeme.ToShortDateString(),
                    Odobrena = r.Odobrena,
                    Preuzeta = r.Preuzeta,
                    RezervacijaID = r.RezervacijaID,
                    VrijemeRezervacije = r.DatumVrijeme.ToShortTimeString()+ " - "+ r.DatumVrijeme.AddHours(r.Trajanje).ToShortTimeString(),
                    Stolovi =  db.RezervacijeStolovi.Include(rs=> rs.Stol).Where(rs=> rs.RezervacijaID == r.RezervacijaID).Select(
                        rs=> rs.Stol.BrojStola).ToList(),
                    Recenzirano = db.Recenzije.Where(rec=> rec.RezervacijaId == r.RezervacijaID).Count()==0 ? false:true,
                    Otkazana = r.Otkazana,
                    Ponistena = r.Ponistena,
                    Komentar = r.Komentar
                }).OrderByDescending(r=> r.RezervacijaID).Take(100).ToListAsync();
        }
        [HttpGet("GetRezervacijeHostesa")]
        public async Task<List<RezervacijaHostesaResponse>> GetRezervacijeHostesa(DateTime datum)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsHostesa) throw new UnauthorizedAccessException();
            return await db.Rezervacije.Include(r => r.Gost.Grad).Where(r=> r.DatumVrijeme.Year == datum.Year &&
            r.DatumVrijeme.Month == datum.Month && r.DatumVrijeme.Day == datum.Day).
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
                    BrojTelefona= r.Gost == null ? r.BrojTelefona! : r.Gost!.BrojTelefona,
                    ImePrezime = r.Gost == null ? r.ImePrezime! : r.Gost!.Ime + " " + r.Gost!.Prezime,
                    Ponistena = r.Ponistena,
                    Otkazana = r.Otkazana
                }).OrderByDescending(r => r.RezervacijaID).ToListAsync();
        }
        [HttpGet("GetRecenzijeManager")]
        public async Task<List<RecenzijaManagerResponse>> GetRecenzijeManager (DateTime datum)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsManager) throw new UnauthorizedAccessException();
            return await db.Recenzije.Include(r=> r.Rezervacija.Gost).Where(r=> r.Rezervacija.DatumVrijeme.Day ==
            datum.Day && r.Rezervacija.DatumVrijeme.Month == datum.Month && r.Rezervacija.DatumVrijeme.Year == datum.Year).
            Select(rec=> new RecenzijaManagerResponse
            {
                RezervacijaId = rec.RezervacijaId,
                BrojTelefona= rec.Rezervacija.Gost.BrojTelefona,
                Datum = rec.Rezervacija.DatumVrijeme.ToShortDateString(),
                Vrijeme = rec.Rezervacija.DatumVrijeme.ToShortTimeString()+ " - " + rec.Rezervacija.DatumVrijeme.AddHours(
                    rec.Rezervacija.Trajanje).ToShortTimeString(),
                ImePrezime= rec.Rezervacija.Gost.Ime +" " + rec.Rezervacija.Gost.Prezime,
                Ocjena = rec.Ocjena,
                RecenzijaId = rec.RecenzijaId
            }
                ).ToListAsync();
        }
        [HttpGet("GetRecenzijaInfoManager")]
        public async Task<RecenzijaInfoManager> GetRecenzijaInfoManager(int id)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsManager) throw new UnauthorizedAccessException();
            var recenzija = await db.Recenzije.Include(rec => rec.Rezervacija.Gost).FirstOrDefaultAsync(rec => rec.RecenzijaId == id);
            if (recenzija == null) { throw new Exception("Recenzija nije pronadjena!"); }
            return new RecenzijaInfoManager
            {
                RezervacijaId = recenzija.RezervacijaId,
                BrojTelefona = recenzija.Rezervacija.Gost.BrojTelefona,
                Datum = recenzija.Rezervacija.DatumVrijeme.ToShortDateString(),
                Vrijeme = recenzija.Rezervacija.DatumVrijeme.ToShortTimeString() + " - " + recenzija.Rezervacija.DatumVrijeme.AddHours(
                    recenzija.Rezervacija.Trajanje).ToShortTimeString(),
                ImePrezime = recenzija.Rezervacija.Gost.Ime + " " + recenzija.Rezervacija.Gost.Prezime,
                Ocjena = recenzija.Ocjena,
                Stolovi = db.RezervacijeStolovi.Include(rs => rs.Stol).Where(rs => rs.RezervacijaID == recenzija.RezervacijaId)
                .Select(rs=> rs.Stol).ToList(),
                Komentar = recenzija.Komentar,
                PosebneZelje = recenzija.Rezervacija.PosebneZelje,
                RecenzijaId= recenzija.RecenzijaId
            };

        }

        [HttpGet("IzbrisiRezervacijuGost")]
        public async Task<bool> IzbrisiRezervaciju(int id)
        {
            var rezervacija = await db.Rezervacije.FindAsync(id);
            if (rezervacija == null) return false;
            if (rezervacija.Preuzeta) return false;
            if (rezervacija.Otkazana) return false;
            if(rezervacija.Ponistena) return false;
            if (DateTime.Now > rezervacija.DatumVrijeme.AddHours(-6)) return false;
            var prijava = await auth.isLogiran();
            if (!prijava.IsGost) throw new UnauthorizedAccessException();
            var gost = await db.Gosti.FindAsync(rezervacija.GostID);
            rezervacija.Otkazana = true;
            db.SaveChanges();
            return true;
        }
        [HttpPost("IzbrisiRezervacijuHostesa")]
        public async Task<bool> IzbrisiRezervacijuHostesa(RezervacijaOtkazHostesa otkaz)
        {
            var rezervacija = await db.Rezervacije.FindAsync(otkaz.RezervacijaId);
            if (rezervacija == null) return false;
            if (rezervacija.Preuzeta) return false;
            if (rezervacija.Otkazana) return false;
            if (rezervacija.Ponistena) return false;
            if (rezervacija.Odobrena) return false;
            var prijava = await auth.isLogiran();
            if (!prijava.IsHostesa && !prijava.IsManager)
            {
                throw new UnauthorizedAccessException();
            }
            if (prijava.IsHostesa &&  DateTime.Now > rezervacija.DatumVrijeme.AddHours(-6) && DateTime.Now < 
                rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje)) return false;
            var gost = await db.Gosti.FindAsync(rezervacija.GostID);
            rezervacija.Ponistena = true;
            rezervacija.Komentar = otkaz.Komentar;
            db.SaveChanges();
            if(DateTime.Now < rezervacija.DatumVrijeme)
            {
                var poruka = $"Vaša rezervacija sa ID:{rezervacija.RezervacijaID} je nažalost odbijena." +
                    $" Molimo posjetite aplikaciju za više informacija.";
                SmsService.posaljiPoruku(gost!.BrojTelefona, poruka);
                var email = $"<h3>Vaša rezervacija sa Id {rezervacija.RezervacijaID} je nažalost odbijena!</h3>" +
                    $"<h4>Razlog: {rezervacija.Komentar}</h4>";
                await smtp.sendEmail("ERezervacije - Odbijanje rezervacije", email, gost!.Email);
            }
            return true;
        }
        [HttpGet("OdobriRezervaciju")]
        public async Task<bool> OdobriRezervaciju (int id)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsHostesa && !prijava.IsManager) throw new UnauthorizedAccessException();
            var rezervacija = await db.Rezervacije.Include(r => r.Gost).Where(r => r.RezervacijaID == id).FirstOrDefaultAsync();
            if (rezervacija == null)return false;
            if(rezervacija.Otkazana || rezervacija.Ponistena || rezervacija.Preuzeta || rezervacija.Odobrena)
            {
                return false;
            }
            if (prijava.IsHostesa && DateTime.Now > rezervacija.DatumVrijeme)
            {
                return false;
            }
            rezervacija.Odobrena = true;
            db.SaveChanges();
            if(DateTime.Now < rezervacija.DatumVrijeme)
            {
                var poruka = $"Vaša rezervacija sa ID:{rezervacija.RezervacijaID} je potvrđena." +
                    $" Čekamo Vas {rezervacija.DatumVrijeme.Day}.{rezervacija.DatumVrijeme.Month}.{rezervacija.DatumVrijeme.Year}. u {rezervacija.DatumVrijeme.Hour} sati.";
                SmsService.posaljiPoruku(rezervacija.Gost.BrojTelefona, poruka);
                var email = $"<h3>Vaša rezervacija sa Id {rezervacija.RezervacijaID} je potvrđena!</h3>" +
                    $"<h4>Čekamo Vas {rezervacija.DatumVrijeme.Day}.{rezervacija.DatumVrijeme.Month}.{rezervacija.DatumVrijeme.Year}. u {rezervacija.DatumVrijeme.Hour} sati.</h4>";
                await smtp.sendEmail("ERezervacije - Potvrda rezervacije", email, rezervacija.Gost.Email);
            }
            return true;
        }
        [HttpPost("KreirajManuelnoRezervaciju")]
        public async Task<RezervacijaResponse> KreirajManuelnoRezervaciju(ManuelnaRezPost rezervacija)
        {
            var prijava = await auth.isLogiran();
            if (prijava.IsGost) throw new UnauthorizedAccessException();
            if(string.IsNullOrWhiteSpace(rezervacija.BrojTelefona) || string.IsNullOrWhiteSpace(rezervacija.ImePrezime))
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Sva polja su obavezna!"
                };
            }
            if(rezervacija.Trajanje<1)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Minimalno vrijeme trajanje rezervacije je 1 sat!"
                };
            }
            if (rezervacija.Stolovi.Count() == 0)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Niste odabrali stolove uz rezervaciju!"
                };
            }
            if (DateTime.Now > rezervacija.DatumVrijeme.AddHours(-6))
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce napraviti maksimalno 6 sati pred pocetak!"
                };
            }
            if (rezervacija.DatumVrijeme.Minute != 0 || rezervacija.DatumVrijeme.Second != 0 ||
              rezervacija.DatumVrijeme.Millisecond != 0)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Neispravan unos satnice rezervacije!"
                };
            }
            if (rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje).Hour > 23 ||
                rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje).Hour < 10)
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce kreirati izmedju 9 i 23 sata!"
                };
            if (rezervacija.DatumVrijeme.Hour > 22 || rezervacija.DatumVrijeme.Hour < 9)
            {
                return new RezervacijaResponse
                {
                    RezervacijaID = 0,
                    Greska = "Rezervaciju je moguce kreirati izmedju 9 i 23 sata!"
                };
            }
            List<int> slobodniStolovi = (await GetSlobodniStolovi(new StoloviRequest
            {
                DatumVrijeme = rezervacija.DatumVrijeme,
                Trajanje = rezervacija.Trajanje
            })).Stolovi.Select(s => s.StolID).ToList();
            foreach (int stol in rezervacija.Stolovi)
            {
                if (!slobodniStolovi.Contains(stol))
                {
                    return new RezervacijaResponse
                    {
                        RezervacijaID = 0,
                        Greska = "Odabrani stolovi vise nisu slobodni!"
                    };
                }
            }
            Rezervacija reservation = new Rezervacija()
            {
                DatumVrijeme = rezervacija.DatumVrijeme,
                Trajanje = rezervacija.Trajanje,
                GostID = null,
                ImePrezime= rezervacija.ImePrezime,
                BrojTelefona= rezervacija.BrojTelefona,
                PosebneZelje = rezervacija.PosebneZelje,
                Komentar = "",
                Odobrena = true,
                Ponistena = false,
                Otkazana = false,
                Preuzeta = false
            };
            db.Rezervacije.Add(reservation);
            db.SaveChanges();
            List<int> stolovi = rezervacija.Stolovi;
            foreach (int brojstola in stolovi)
            {
                RezervacijaStol rezervacijastol = new RezervacijaStol()
                {
                    RezervacijaID = reservation.RezervacijaID,
                    StolID = brojstola
                };
                db.RezervacijeStolovi.Add(rezervacijastol);
                db.SaveChanges();
            }
            return new RezervacijaResponse()
            {
                RezervacijaID = reservation.RezervacijaID,
                Greska = ""
            };

        }
        [HttpGet("OznaciPreuzetom")]
        public async Task<bool> OznaciPreuzetom(int id)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.IsHostesa && !prijava.IsManager) throw new UnauthorizedAccessException();
            var rezervacija = await db.Rezervacije.Include(r => r.Gost).Where(r=> r.RezervacijaID == id).FirstOrDefaultAsync();
            if (rezervacija == null) return false;
            if (rezervacija.Ponistena || rezervacija.Otkazana || rezervacija.Preuzeta || !rezervacija.Odobrena) return false;
            if (prijava.IsHostesa && DateTime.Now > rezervacija.DatumVrijeme.AddHours(rezervacija.Trajanje)) return false;
            rezervacija.Preuzeta= true;
            if (rezervacija.Gost != null)
            {
                if ((rezervacija.Gost.BrojRezervacija + 1) % 10 == 0)
                {
                    var popust = new GostPopust
                    {
                        PopustID = 1,
                        GostID = rezervacija.GostID ?? 0,
                        Iskoristen = false
                    };
                    db.GostiPopusti.Add(popust);
                }
                rezervacija.Gost.BrojRezervacija = rezervacija.Gost.BrojRezervacija + 1;
            }
            db.SaveChanges();
            return true;
        }
        [HttpGet("GetGostRezervacijaInfo")]
        public async Task<RezervacijaGostEdit> GetGostRezervacijaInfo (int id)
        {
            var rezervacija = await db.Rezervacije.Include(r => r.Gost).FirstOrDefaultAsync(r=> r.RezervacijaID == id);
            if (rezervacija == null) throw new Exception();
            var prijava = await auth.isLogiran();
            if (prijava.IsGost)
            {
                if (rezervacija.GostID != prijava.Prijava!.KorisnikKorisnikID) throw new UnauthorizedAccessException();
            }
            List<Stol> stolovi = await db.RezervacijeStolovi.Include(rs=> rs.Stol).Where(rs => rs.RezervacijaID == rezervacija.RezervacijaID).
                Select(rs=> rs.Stol).ToListAsync();
            return new RezervacijaGostEdit
            {
                RezervacijaId = id,
                Datum = rezervacija.DatumVrijeme.Date,
                Vrijeme = rezervacija.DatumVrijeme.Hour,
                PosebneZelje = rezervacija.PosebneZelje,
                Stolovi = stolovi,
                Trajanje = rezervacija.Trajanje,
                Ponistena = rezervacija.Ponistena,
                Komentar = rezervacija.Komentar
            };
        }

        [HttpPost ("DodajRecenziju")]
        public async Task<bool> DodajRecenziju (RecenzijaPost recenzija)
        {
            if (recenzija.Ocjena < 1 || recenzija.Ocjena > 5) return false;
            if (db.Recenzije.Where(r => r.RecenzijaId == recenzija.RezervacijaId).Count() != 0) return false;
            var rezervacija = await  db.Rezervacije.Include(r=> r.Gost).FirstOrDefaultAsync(r=> r.RezervacijaID == recenzija.RezervacijaId);
            if (rezervacija == null) return false;
            if (!rezervacija.Preuzeta) return false;
            var prijava = await auth.isLogiran();
            if (rezervacija.GostID != prijava.Prijava!.KorisnikKorisnikID) throw new UnauthorizedAccessException();
            try
            {
                var nova = new Recenzija
                {
                    RezervacijaId = recenzija.RezervacijaId,
                    Komentar = recenzija.Komentar,
                    Ocjena = recenzija.Ocjena
                };
                db.Recenzije.Add(nova);
                db.SaveChanges();
            }catch(Exception ex)
            {
                return false;
            }
            return true;
        }



        [HttpPost("GetSlobodniStolovi")]
        public async Task<StoloviResponse> GetSlobodniStolovi
            ([FromBody] StoloviRequest request)
        {
            if(request.DatumVrijeme < DateTime.Now.AddHours(6))
            {
                return new StoloviResponse()
                {
                    Stolovi = new List<Stol> { }
                };
            }
            if(request.DatumVrijeme > DateTime.Now.AddDays(5))
            {
                return new StoloviResponse()
                {
                    Stolovi = new List<Stol> { }
                };
            }

            List<Stol> stolovi = await db.Stolovi.ToListAsync();
                List<Stol> zauzeti = db.RezervacijeStolovi
                    .Include(rs=> rs.Stol).Include(rs=> rs.Rezervacija).Where(
                    rs=> !rs.Rezervacija.Ponistena && !rs.Rezervacija.Otkazana &&  ((rs.Rezervacija.DatumVrijeme >= request.DatumVrijeme &&
                    rs.Rezervacija.DatumVrijeme < request.DatumVrijeme.AddHours(request.Trajanje)) || (request.DatumVrijeme >= rs.Rezervacija.DatumVrijeme &&
                    request.DatumVrijeme < rs.Rezervacija.DatumVrijeme.AddHours(rs.Rezervacija.Trajanje)))
                    ).
                   Select(
                      rs => rs.Stol
                    ).ToList();
            List<Stol> zabranjeni = db.ZabranaStol.Include(zs => zs.Stol).Where(
                zs => (zs.Zabrana.PocetakVrijeme >= request.DatumVrijeme && 
                zs.Zabrana.PocetakVrijeme < request.DatumVrijeme.AddHours(request.Trajanje)) 
                || (request.DatumVrijeme >= zs.Zabrana.PocetakVrijeme &&
               request.DatumVrijeme < zs.Zabrana.KrajVrijeme)).Select(zs => zs.Stol).ToList();
            List<Stol> slobodniStolovi = new List<Stol>();
            foreach(Stol stol in stolovi)
            {
                if(!zauzeti.Contains(stol) && !zabranjeni.Contains(stol)){
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
