using ERezervacijeAPI.Data;
using ERezervacijeAPI.Klase;
using Microsoft.AspNetCore.Mvc;

namespace ERezervacijeAPI.Controllers
{
    [ApiController]
    public class StoloviController
    {
        public readonly DBConnection db;
        public StoloviController(DBConnection dBConnection)
        {
            db = dBConnection;
        }
        [HttpGet("getStolovi")]
        public List<Stol> getStolovi()
        {
            return db.Stolovi.ToList();
        }
 
    }
}
