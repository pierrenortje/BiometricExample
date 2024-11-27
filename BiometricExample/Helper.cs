using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace BiometricExample
{
    public static class Helper
    {
        public static string GenerateChallenge()
        {
            var challenge = new byte[32];
            RandomNumberGenerator.Fill(challenge);
            return Base64UrlEncoder.Encode(challenge);
        }
    }
}
