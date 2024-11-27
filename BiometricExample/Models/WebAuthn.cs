namespace BiometricExample.Models
{
    public class WebAuthnUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Challenge { get; set; }
        public string PublicKey { get; set; }
        public WebAuthnCredential Credential { get; set; }
    }

    public class WebAuthnCredential
    {
        public string Id { get; set; }
        public string RawId { get; set; }
        public string Type { get; set; }
        public WebAuthnResponse Response { get; set; }
    }

    public class WebAuthnResponse
    {
        public string ClientDataJSON { get; set; }
        public string AttestationObject { get; set; }
    }

    public class WebAuthnAssertion
    {
        public string Id { get; set; }
        public string RawId { get; set; }
        public string Type { get; set; }
        public WebAuthnLoginResponse Response { get; set; }
    }

    public class WebAuthnLoginResponse
    {
        public string ClientDataJSON { get; set; }
        public string AuthenticatorData { get; set; }
        public string Signature { get; set; }
        public string UserHandle { get; set; }
    }
}
