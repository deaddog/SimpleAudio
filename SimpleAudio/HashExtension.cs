using System.Security.Cryptography;
using System.Text;

namespace SimpleAudio
{
    public static class HashExtension
    {
        public static string GetHashString(this HashAlgorithm algorithm, string input)
        {
            if (input == null || input.Length == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (var b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
    }
}
