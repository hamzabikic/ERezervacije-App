namespace ERezervacijeAPI.Helpers
{
    public class UserKeyGenerator
    {
        public static string GenerisiKey()
        {
            var slova = "1234567890qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            var key = "";
            var random = new Random();
            for (int i = 0; i < 15; i++)
            {
                key += slova[random.Next(0, slova.Length - 1)];
            }
            return key;
        }
        public static string GenerisiPhoneKey()
        {
            var slova = "1234567890qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            var key = "";
            var random = new Random();
            for (int i = 0; i < 6; i++)
            {
                key += slova[random.Next(0, slova.Length - 1)];
            }
            return key;
        }
    }
}
