using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace SecureMauiApp.Shared.Model
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [PasswordPropertyText(true)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        public string LoginFailureMessage { get; set; } = "Invalid Email or Password. Please try again.";

    }
    public class DecodedToken
    {
        private string keyId;
        private string issuer;
        private IEnumerable<string> audience;
        public IEnumerable<Claim> Claims { get; private set; }
        private DateTime validTo;
        private string signatureAlgorithm;
        private string rawData;
        private string subject;
        private DateTime validFrom;
        private string encodedHeader;
        private string encodedPayload;

        public DecodedToken(string keyId, string issuer, IEnumerable<string> audience, IEnumerable<Claim> claims, DateTime validTo, string signatureAlgorithm, string rawData, string subject, DateTime validFrom, string encodedHeader, string encodedPayload)
        {
            this.keyId = keyId;
            this.issuer = issuer;
            this.audience = audience;
            this.Claims = claims;
            this.validTo = validTo;
            this.signatureAlgorithm = signatureAlgorithm;
            this.rawData = rawData;
            this.subject = subject;
            this.validFrom = validFrom;
            this.encodedHeader = encodedHeader;
            this.encodedPayload = encodedPayload;
        }
    }
    public enum LoginStatus
    {
        None,
        Success,
        Failed
    }
}
