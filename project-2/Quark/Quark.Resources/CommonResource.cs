using Microsoft.Extensions.Localization;

namespace Quark.Resources;

public interface ICommonResource
{
}

public class CommonResource : ICommonResource
{
    private readonly IStringLocalizer _localizer;
    public CommonResource(IStringLocalizer<CommonResource> localizer)
    {
        _localizer = localizer;
    }
    public string this[string index]
    {
        get
        {
            return _localizer[index];
        }
    }
}