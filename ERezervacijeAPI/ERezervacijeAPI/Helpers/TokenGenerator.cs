using ERezervacijeAPI.Klase;

namespace ERezervacijeAPI.Helpers
{
    public class TokenGenerator
    {
        public static string GenerisiToken()
        {
            var slova = "1234567890qwertzuiopasdfghjklyxcvbnm,.-<!#$%&/()=?*+";
            var token = "";
            var random = new Random();
            for(int i =0; i<8;i++)
            {
                token += slova[random.Next(0, slova.Length - 1)];
            }
            return token;
        }
    }
}
