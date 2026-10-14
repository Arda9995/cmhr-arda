using AvvaMobile.Core.Business;
using AvvaMobile.Core.Handlers.JwtToken;
using Quark.API.Business.Models.Requests.AccountRequests;
using Quark.API.Business.Models.Responses.AccountResponses;
using Quark.API.Business.Models.Responses.LoginResponses;

namespace Quark.API.Business.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult> LoginAsync(LoginRequest request);

        Task<ServiceResult<Token>> LoginEmailVerificationAsync(SmsVerificationRequest request);

        ServiceResult<Token> GetAnonymousToken(GetAnonymousTokenRequest request);
        
        // New methods
        Task<ServiceResult<EmailLoginResponse>> EmailLoginAsync(EmailLoginRequest request);
        
        Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request);
        
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordRequest request);
        
        Task<ServiceResult> ChangePasswordAsync(ChangePasswordRequest request);
        
        Task<ServiceResult<AccountMainPageResponse>> GetAccountInformationAsync();
        
        Task<ServiceResult> UpdateAccountInformationAsync(AccountMainPageResponse request);
        
        Task<ServiceResult> CancelPremiumMembershipRenewalAsync();
        
        // Registration method
        Task<ServiceResult> AccountInformations(RegistrationRequest request);
        Task<ServiceResult<GetMembershipIntroductionResponse>> MembershipIntroduction();
        Task<ServiceResult<RegistrationRequest>> AccountInformations();

		Task<ServiceResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request);
		Task<ServiceResult> RegisterAsync();

		// Logout method
		Task<ServiceResult> LogoutAsync();
    }
}