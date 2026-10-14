using AvvaMobile.Core.Business;
using Quark.Data.Models.Common;

namespace Quark.API.Business.Interfaces
{
    public interface ISlugService
    {
        Task<ServiceResult<EntityTypeEnum>> GetSlugInfoAsync(string slug);
    }
}
