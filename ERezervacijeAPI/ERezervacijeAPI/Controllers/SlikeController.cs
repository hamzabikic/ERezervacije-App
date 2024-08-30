using ERezervacijeAPI.Data;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.ManagerKlase;
using ERezervacijeAPI.Services;
using ERezervacijeAPI.Slike;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    public class SlikeController : ControllerBase
    {
        private readonly DBConnection db;
        private readonly MyAuthService auth;
        private readonly IWebHostEnvironment env;
        public SlikeController(DBConnection _db,MyAuthService _auth, IWebHostEnvironment _env)
        {
            db = _db;
            auth = _auth;
            env = _env;
        }
        [HttpGet("GetLogo")]
        public async Task<ActionResult> GetLogo()
        {
            byte[] logo = System.IO.File.ReadAllBytes("logo/logo.png");
            return File(logo, "image/png");
        }
        [HttpGet("GetProfilnaById")]
        public async Task<FileContentResult> GetProfilnaById (int id)
        {
            byte[] slika = new byte[0];
            var korisnik = await db.Korisnici.FindAsync(id);
            if (korisnik == null || korisnik.Slika == null || korisnik.Slika=="") return File(slika, "image/jpeg", "profilna.jpg");
            try
            {
                slika = System.IO.File.ReadAllBytes(korisnik.Slika);
            }catch(Exception)
            {
                return File(new byte[0], "image/jpeg", "profilna.jpg");
            }
            return File(slika, "image/jpeg", "profilna.jpg");
        }
        [HttpPost("SendProfilna")]
        public async Task<KorisnikEditResponse> SendProfilna(SlikaPost post)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.JePrijavljen) throw new UnauthorizedAccessException();
            if(prijava.IsHostesa || (prijava.IsGost && prijava.Prijava!.KorisnikKorisnikID!=post.KorisnikId))
            {
                throw new UnauthorizedAccessException();
            }
            var korisnik = await db.Korisnici.FindAsync(post.KorisnikId);
            if (korisnik == null) return new KorisnikEditResponse { Editovan = false, Greska = "Korisnik nije pronadjen u sistemu!" };
            var putanja = "profilne-slike/"+korisnik.KorisnikID+".jpg";
            if(!Directory.Exists("profilne-slike"))
            {
                Directory.CreateDirectory("profilne-slike");
            }
            var base64String = post.Base64String.Split(",")[1];
            try
            {
                var slika = Convert.FromBase64String(base64String);
                await System.IO.File.WriteAllBytesAsync(putanja, slika);
            }catch(Exception ex)
            {
                return new KorisnikEditResponse { Editovan =false, Greska = "Neuspjesna pohrana slike!" };
            }
            korisnik.Slika = putanja;
            db.SaveChanges();
            return new KorisnikEditResponse { Editovan = true, Greska = "" };
        }

        [HttpGet("CheckControllersDirectory")]
        public IActionResult CheckControllersDirectory()
        {
            var contentRootPath = env.ContentRootPath;
            Console.WriteLine($"Content Root Path: {contentRootPath}");

            // Log all files and directories in the current directory
            var filesAndDirs = Directory.GetFileSystemEntries(contentRootPath);
            foreach (var entry in filesAndDirs)
            {
                Console.WriteLine(entry);
            }

            var controllersPath = Path.Combine(contentRootPath, "Controllers");
            bool directoryExists = Directory.Exists(controllersPath);

            return Ok(new { path = controllersPath, directoryExists, filesAndDirs });
        }



    }
}
