using System.Security.Cryptography;
using System.Text;

namespace LUMI.DataAccess;

public static class Hashing {
    public static string StringToHash(this string value) {
        using (var sha128 = SHA1.Create()) {
            var _bytes = Encoding.UTF8.GetBytes(value);
            var _hash = sha128.ComputeHash(_bytes);
            return Convert.ToBase64String(_hash);
        }
    }
    public static string HashToString (this string value){
        using (var sha1 = SHA1.Create()) {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha1.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}