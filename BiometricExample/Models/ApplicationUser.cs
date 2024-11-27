using Microsoft.AspNetCore.Identity;

namespace BiometricExample.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Challenge { get; set; }
        public string PublicKey { get; set; }
        public string FirstName { get; set; }
        public string CredentialId { get; set; }
    }
}
