using ERezervacijeAPI.Data;
using ERezervacijeAPI.ManagerKlase;
using ERezervacijeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ERezervacijeAPI.Helpers
{
    public class CheckAttributesHelper
    {
        private readonly DBConnection db;
        private readonly MyAuthService auth;
        public CheckAttributesHelper(DBConnection _db, MyAuthService _auth)
        {
            db = _db;
            auth = _auth;
        }
        public async Task<bool> PostojiUsername(string username, string currently)
        {
            if (username == currently) return false;
            if (await db.Korisnici.Where(k => k.Username == username).CountAsync() == 0)
                return false;
            return true;
        }
        public async Task<bool> PostojiEmail(string email,string currently)
        {
            if (email == currently) return false;
            if (await db.Korisnici.Where(k => k.Email == email).CountAsync() == 0 && await
                db.BlokiraniKorisnici.Where(bk=> bk.Email == email).CountAsync() ==0)
                return false;
            return true;
        }
        public async Task<bool> PostojiBrojTelefona(string telefon, string currently)
        {
            if (telefon == currently) return false;
            if (await db.Korisnici.Where(k => k.BrojTelefona == telefon).CountAsync() == 0 &&
                await db.BlokiraniKorisnici.Where(bk=> bk.BrojTelefona == telefon).CountAsync() ==0)
                return false;
            return true;
        }
        public static bool IspravanEmail (string email)
        {
            string EmailRegex = @"^[a-zA-Z0-9]([a-zA-Z0-9]*\.?[a-zA-Z0-9]+)*@[a-zA-Z0-9]([a-zA-Z0-9]*\.?[a-zA-Z0-9]+){0,2}[a-zA-Z0-9]*\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, EmailRegex);
        }
        public static bool IspravanTelefon (string telefon)
        {
            string TelefonRegex = @"^\+\d{1,3}\d{8,10}$";

            return Regex.IsMatch(telefon, TelefonRegex);
        }
    }
}
