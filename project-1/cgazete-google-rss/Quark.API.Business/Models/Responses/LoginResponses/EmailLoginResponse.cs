using AvvaMobile.Core.Handlers.JwtToken;

namespace Quark.API.Business.Models.Responses.LoginResponses
{
    public class EmailLoginResponse
    {
        public bool IsNewUser { get; set; }
        public Token Token { get; set; }
        public bool HasPremiumMembership { get; set; }
        public DateTime? PremiumMembershipExpirationDate { get; set; }
        public int RemainingDays { get; set; }
        public int MemberID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
} 