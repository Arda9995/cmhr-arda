using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.SourceResponses;

namespace Quark.API.Business.Interfaces
{
    public interface ISourceServices
    {
        Task<ServiceResult<List<GetSourceResponse>>> List();
    }
}
