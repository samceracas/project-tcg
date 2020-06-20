using System.Linq;

namespace CardGame.Utils
{
    class Random
    {
        private static Randomizer random = new Randomizer();

        public static string RandomString(int length)
        {
            return random.RandomString(length);
        }

        public static int RandomInt(int min, int max)
        {
            return random.RandomInt(min, max);
        }
    }

    public class Randomizer
    {
        private System.Random _random;

        public Randomizer(string seed)
        {
            _random = new System.Random(seed.GetHashCode());
        }

        public Randomizer()
        {
            _random = new System.Random();
        }

        public string RandomString(int length)
        {
            const string chars = "asdfghjklzxcvbnmqwertyuiopABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public int RandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
