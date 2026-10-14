using Microsoft.AspNetCore.SignalR;
using Quark.CMS.Business.Interfaces;

namespace Quark.CMS.Business.Hubs;

public class SignalRHub : Hub
{
    private readonly IAIService _aiService;

    public SignalRHub(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task SendContentToAi(string content)
    {
        var result = _aiService.Summarize(content);

        await foreach (var complation in result)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetPromptResult", complation);
        }
    }
    
    public async Task SendTitleGenerationRequestToAi(int promptId, string subtitle, string content, string tags, string title)
    {
        var result = _aiService.Title(promptId, subtitle, content, tags, title);
        
        await foreach (var completion in result)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetTitleGenerationResult", completion);
        }
    }
    
    public async Task SendSubtitleGenerationRequestToAi(int promptId, string title, string content, string tags, string subtitle)
    {
        var result = _aiService.Subtitle(promptId, title, content, tags, subtitle);
        
        await foreach (var completion in result)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetSubtitleGenerationResult", completion);
        }
    }
    
}