using AvvaMobile.Core.Business;
using Quark.Data.Models;
using Quark.Data.Models.Payment;

namespace Quark.Business.Caching.Interfaces
{
    public interface IAccountCachingService
    {
        public Task<ServiceResult<List<Member>>> GetMembers();
        public Task<ServiceResult<List<Plan>>> GetPlans();
        public Task<ServiceResult> RefreshPlans();
        public Task<ServiceResult> RefreshMembers();
    }
}
