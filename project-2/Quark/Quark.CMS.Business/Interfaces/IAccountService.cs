using AvvaMobile.Core.Business;
using Quark.CMS.Business.Models.Requests.AccountRequests;
using Quark.CMS.Business.Models.Responses.AccountResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult<SignInResponse>> SignInAsync(SignInRequest request);        
        Task<ServiceResult> SignInOTPAsync(SignInOTPRequest request);
        Task<ServiceResult> SignOutAsync();
        Task<ServiceResult<UpdateProfileRequest>> GetProfile();        
        Task<ServiceResult> UpdateProfile(UpdateProfileRequest request);
    }
}