using Microsoft.AspNetCore.Routing;

namespace Quark.Web.Middlewares.Interfaces
{
	public interface IUrlMappingService
	{
		void RefreshUrlMappingsCache();
	}
}
