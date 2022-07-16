namespace Net.DJDole
{
    using System.Security.Cryptography;
    using System.Text;

    class Utils
    {
        public static string ComputeHash(string input)
        {
            byte[] hash = (SHA1.Create()).ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) { sb.Append(b.ToString("X2")); }
            return sb.ToString();
        }
    }
}