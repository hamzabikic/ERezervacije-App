using ERezervacijeAPI.Data;
using ERezervacijeAPI.Klase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ERezervacijeAPI.Services
{
    public class MyAuthService
    {
        private readonly IHttpContextAccessor http;
        private readonly DBConnection DB;
        public MyAuthService(IHttpContextAccessor _http, DBConnection db)
        {
            http = _http;
            DB = db;
        }
        public async Task<AuthObject> isLogiran()
        {
            string token = "";
            try
            {
                token = http.HttpContext.Request.Headers["my-token"][0];
            }catch(Exception)
            {
                return new AuthObject()
                {
                    JePrijavljen = false,
                    Prijava = null,
                    IsGost = false,
                    IsHostesa = false,
                    IsManager = false
                };
            }
            var prijava = await DB.Prijave.Include(p => p.Korisnik.Grad).Where(p => p.Token == token).FirstOrDefaultAsync();
            if(prijava == null)
            {
                return new AuthObject()
                {
                    JePrijavljen = false,
                    Prijava = null,
                    IsGost = false,
                    IsHostesa = false,
                    IsManager = false
                };
            }
            else
            {
                return new AuthObject()
                {
                    JePrijavljen = true,
                    Prijava = prijava,
                    IsGost = prijava.Korisnik.isGost,
                    IsHostesa = prijava.Korisnik.isHostesa,
                    IsManager = prijava.Korisnik.isManager
                };
            }
        }
    }
    public class AuthObject { 
        public bool JePrijavljen { get; set; }
        public Prijava? Prijava{ get; set; }
        public bool IsGost { get; set; }
        public bool IsHostesa { get; set; }
        public bool IsManager { get; set; }

    }
}
