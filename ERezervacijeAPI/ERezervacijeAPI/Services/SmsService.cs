using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ERezervacijeAPI.Services
{
    public class SmsService
    {
        public static void posaljiPoruku (string phoneNumber, string text)
        {
            //ovdje ide kod za slanje poruke preko twilio servisa koji mozete pronaci u txt fajlu (twilioKod.txt)
        }
    }
}
