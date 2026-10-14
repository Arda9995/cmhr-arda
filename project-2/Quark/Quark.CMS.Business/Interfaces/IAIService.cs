using AvvaMobile.Core.Business;

namespace Quark.CMS.Business.Interfaces;

public interface IAIService
{
    IAsyncEnumerable<string> Summarize(string content);
    IAsyncEnumerable<string> Title(int promptId, string subtitle, string content, string tags, string title);
    IAsyncEnumerable<string> Subtitle(int promptId, string title, string content, string tags, string subtitle);
}