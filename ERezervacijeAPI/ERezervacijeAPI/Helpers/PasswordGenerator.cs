namespace ERezervacijeAPI.Helpers
{
    public class PasswordGenerator
    {
        public static string generisiLozinku()
        {
            var slova = "1234567890qwertzuiopasdfghjklyxcvbnm,.-<!#$%&/()=?*+";
            var lozinka = "";
            var random = new Random();
            for(int i =0; i<8; i++)
            {
                lozinka += slova[random.Next(0, slova.Length)];
            }
            return lozinka;
        }
    }
}
