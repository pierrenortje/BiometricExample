using BiometricExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeterO.Cbor;

namespace BiometricExample.Controllers
{
    public class BioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BioController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/register-challenge")]
        public async Task<IActionResult> GetRegisterChallenge()
        {
            var usr = await _userManager.FindByNameAsync(User.Identity.Name);
            if (string.IsNullOrEmpty(usr.FirstName))
                usr.FirstName = "Gandalf"; // Just for demo purposes!

            var user = new WebAuthnUser
            {
                Id = usr.Id,
                Name = usr.FirstName,
                DisplayName = usr.FirstName
            };

            // Generate a register challenge
            var challenge = Helper.GenerateChallenge();
            user.Challenge = challenge;

            // Persist user challenge
            usr.Challenge = challenge;
            await _userManager.UpdateAsync(usr);

            var response = new
            {
                challenge,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    displayName = user.DisplayName
                }
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] WebAuthnCredential credential)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Convert from base64Url to base64 and then to a byte array
            var attestationObject = Convert.FromBase64String(credential.Response.AttestationObject.Base64UrlToBase64()); // Base64-encoded attestationObject

            // Decode the attestation object
            var attestationCbor = CBORObject.DecodeFromBytes(attestationObject);

            // Extract authData from the attestation object
            byte[] authData = attestationCbor["authData"].GetByteString();

            // Extract the public key (COSE Key format)
            byte[] publicKeyCose = Encryption.Helper.ExtractCoseKey(authData);

            // Parse the COSE Key to extract x and y
            (byte[] x, byte[] y) = Encryption.Helper.ExtractXYFromCoseKey(publicKeyCose);

            // Combine the X and the Y into one array
            byte[] combinedArr = Encryption.Helper.CombineArrays(x, y);

            // Convert to HEX so that we can persist it in the database
            var publicKey = Convert.ToHexString(combinedArr);

            // Persist
            user.CredentialId = credential.Id;
            user.PublicKey = publicKey;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet("verify-challenge")]
        public async Task<IActionResult> GetVerifyChallenge()
        {
            // Persist new challenge
            var challenge = Helper.GenerateChallenge();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Challenge = challenge;
            await _userManager.UpdateAsync(user);

            var allowCredentials = new List<object> { new { id = user.CredentialId } };
            var response = new
            {
                challenge,
                allowCredentials
            };

            return Ok(response);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] WebAuthnAssertion assertion)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (Encryption.Helper.VerifySignature(assertion, user.PublicKey))
                return Ok("Login successful!");

            return BadRequest("Invalid signature");
        }
    }
}
