using ERezervacijeAPI.Data;
using ERezervacijeAPI.Klase;
using ERezervacijeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    public class GradoviController
    {
        private readonly DBConnection db;
        private readonly SmtpService smtp;
        public GradoviController(DBConnection _db, SmtpService _smtp)
        {
            db = _db;
            smtp = _smtp;
        }
        [HttpGet("GetGradovi")]
        public async Task<List<Grad>> getGradovi()
        {
            return await db.Gradovi.ToListAsync();
        }
    }
}
