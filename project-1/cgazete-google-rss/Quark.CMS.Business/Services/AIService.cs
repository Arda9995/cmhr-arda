using System.Text;
using AvvaMobile.Core;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class AIService: BaseService, IAIService
{
    private readonly IOpenAIService _openAIService;
    private readonly IPromptCachingService _promptCachingService;
    public AIService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IOpenAIService openAIService, IPromptCachingService promptCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _openAIService = openAIService;
        _promptCachingService = promptCachingService;
    }

    //TODO: UI Entegrasyonu yapıldıktan sonra Summarize methodu kaldırılacak
    public async IAsyncEnumerable<string> Summarize(string content)
    {
        var completionResult = _openAIService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
        {
            Messages = [
                new(StaticValues.ChatMessageRoles.System, "Sen bir haber editörüsün."),
                new(StaticValues.ChatMessageRoles.User, content)
            ],
            Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_16k
        });
        
        await foreach (var completion in completionResult)
        {
            if (completion.Successful)
            {
                yield return completion.Choices.First().Message.Content;
            }
            else
            {
                if (completion.Error == null)
                {
                    throw new Exception("Unknown Error");
                }

                yield return $"{completion.Error.Code}: {completion.Error.Message}";
            }
        }
    }

    public async IAsyncEnumerable<string> Title(int promptId, string subtitle, string content, string tags, string title)
    {
        // var prompt = await _promptCachingService.GetPromptByID(promptId);
        var prompt = await _context.Prompts.FirstOrDefaultAsync(x => x.ID == promptId);

        #region Prompt Builder
        var sb = new StringBuilder();
        sb.Append(prompt.Message);
        sb.Append("\n");
        if (!string.IsNullOrEmpty(subtitle))
        {
            sb.Append("Alt Başlık: ");
            sb.Append(subtitle);
            sb.Append("\n");
        }
        if (!string.IsNullOrEmpty(content))
        {
            sb.Append("İçerik: ");
            sb.Append(content);
            sb.Append("\n");
        }
        if (!string.IsNullOrEmpty(tags))
        {
            sb.Append("Etiketler: ");
            var formattedTags = tags.Replace("[{\"value\":\"", "").Replace("\"},{\"value\":\"", ",").Replace("\"}]", "");
            sb.Append(formattedTags);
        }
        if (!string.IsNullOrEmpty(title))
        {
            sb.Append("Ana Başlık: ");
            sb.Append(title);
            sb.Append("\n");
        }
        var finalPrompt = sb.ToString();
        #endregion

        #region Rules
        var rulesSb = new StringBuilder();
        rulesSb.Append("Uyman gereken kurallar:\n");
        rulesSb.Append("1- Cevap olarak sadece ana başlığın kendisini dönmelisin.\n");
        rulesSb.Append("2- 'Ana Başlık:' gibi ifadelerle başlamamalı.\n");
        rulesSb.Append("3- Cevap alıntı olarak dönmemeli.");
        var rules = rulesSb.ToString();
        #endregion

        var trainPrompt = GetTitleTrainPrompt(prompt.Message);
        var trainResult = "Antalya Büyükşehir Belediye Başkan adayları kim? 31 Mart yerel seçimlerinde Antalya Büyükşehir'den kaç kişi belediye başkan adayı oldu? Antalya Büyükşehir'de hangi parti kimi aday gösterdi?";
        
        var completionResult = _openAIService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
        {
            Messages = [
                new(StaticValues.ChatMessageRoles.System, "Sen bir haber editörüsün."),
                new(StaticValues.ChatMessageRoles.System, rules),
                new(StaticValues.ChatMessageRoles.User, trainPrompt),
                new(StaticValues.ChatMessageRoles.Assistant, trainResult),
                new(StaticValues.ChatMessageRoles.User, finalPrompt)
            ],
            Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_16k
        });
        
        await foreach (var completion in completionResult)
        {
            if (completion.Successful)
            {
                if (completion.Choices.First().Message.Content != null)
                {
                    yield return completion.Choices.First().Message.Content;
                }
            }
            else
            {
                if (completion.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
        
                yield return $"{completion.Error.Code}: {completion.Error.Message}";
            }
        }    

        // var testResult = AiTestResponse();
        //
        // await foreach (var completion in testResult)
        // {
        //     yield return completion;
        // }
    }

    public async IAsyncEnumerable<string> Subtitle(int promptId, string title, string content, string tags, string subtitle)
    {
        // var prompt = await _promptCachingService.GetPromptByID(promptId);
        var prompt = await _context.Prompts.FirstOrDefaultAsync(x => x.ID == promptId);

        #region Prompt Builder
        var sb = new StringBuilder();
        sb.Append(prompt.Message);
        sb.Append("\n");
        if (!string.IsNullOrEmpty(subtitle))
        {
            sb.Append("Alt Başlık: ");
            sb.Append(subtitle);
            sb.Append("\n");
        }
        if (!string.IsNullOrEmpty(content))
        {
            sb.Append("İçerik: ");
            sb.Append(content);
            sb.Append("\n");
        }
        if (!string.IsNullOrEmpty(tags))
        {
            sb.Append("Etiketler: ");
            var formattedTags = tags.Replace("[{\"value\":\"", "").Replace("\"},{\"value\":\"", ",").Replace("\"}]", "");
            sb.Append(formattedTags);
        }
        if (!string.IsNullOrEmpty(title))
        {
            sb.Append("Ana Başlık: ");
            sb.Append(title);
            sb.Append("\n");
        }
        var finalPrompt = sb.ToString();
        #endregion

        #region Rules
        var rulesSb = new StringBuilder();
        rulesSb.Append("Uyman gereken kurallar:\n");
        rulesSb.Append("1- Cevap olarak sadece alt başlığın kendisini dönmelisin.\n");
        rulesSb.Append("2- 'Alt Başlık:' gibi ifadelerle başlamamalı.\n");
        rulesSb.Append("3- Cevap alıntı olarak dönmemeli.");
        var rules = rulesSb.ToString();
        #endregion

        var trainPrompt = GetSubtitleTrainPrompt(prompt.Message);
        var trainResult = "31 Mart'ta yapılacak Mahalli İdareler Genel Seçimleri'nde yarışacak belediye başkan adayları belli oldu. Antalya Büyükşehir Büyükşehir Belediye Başkanlığı için 22'si siyasi parti, 6'sı bağımsız toplam 28 aday seçime katılacak.";
        
        
        var completionResult = _openAIService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
        {
            Messages = [
                new(StaticValues.ChatMessageRoles.System, "Sen bir haber editörüsün."),
                new(StaticValues.ChatMessageRoles.System, rules),
                new(StaticValues.ChatMessageRoles.User, trainPrompt),
                new(StaticValues.ChatMessageRoles.Assistant, trainResult),
                new(StaticValues.ChatMessageRoles.User, finalPrompt)
            ],
            Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_16k
        });
        
        await foreach (var completion in completionResult)
        {
            if (completion.Successful)
            {
                if (completion.Choices.First().Message.Content != null)
                {
                    yield return completion.Choices.First().Message.Content;
                }
            }
            else
            {
                if (completion.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
        
                yield return $"{completion.Error.Code}: {completion.Error.Message}";
            }
        }   
    }
    
    private async IAsyncEnumerable<string> AiTestResponse()
    {
        for (int i = 0; i < 15; i++)
        {
            yield return KeyGenerator.CreateAlphanumericCode(10);
        }
    }
    
    private string GetTitleTrainPrompt(string prompt)
    {
        var sb = new StringBuilder();
        sb.Append(prompt);
        sb.Append("\n");
        
        sb.Append("Alt Başlık: ");
        sb.Append("31 Mart'ta yapılacak Mahalli İdareler Genel Seçimleri'nde yarışacak belediye başkan adayları belli oldu. Antalya Büyükşehir Büyükşehir Belediye Başkanlığı için 22'si siyasi parti, 6'sı bağımsız toplam 28 aday seçime katılacak.");
        sb.Append("\n");
        
        sb.Append("İçerik: ");
        sb.Append("<p><a title=\"31 Mart 2024 haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/31-mart-2024\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">31 Mart 2024</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">'te yapılacak olan&nbsp;</span><a title=\"yerel seçimler haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/yerel-secimler\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">yerel seçimler</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">de yarışacak olan adaylar belli oldu.</span><a title=\"Antalya haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/antalya\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">Antalya</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">&nbsp;Büyükşehir Büyükşehir Belediye Başkanlığı için 22'si siyasi parti, 6'sı bağımsız toplam 28 aday seçime katılacak. Peki,</span><strong style=\"overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; color: rgb(34, 34, 34); font-size: 19.2px;\">&nbsp;Antalya Büyükşehir Belediye Başkan adayları kim? 31 Mart yerel seçimlerinde Antalya'dan kaç kişi&nbsp;<a title=\"belediye başkan adayı haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/belediye-baskan-adayi\" style=\"color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif !important;\">belediye başkan adayı</a>&nbsp;oldu? Antalya'da hangi parti kimi aday gösterdi?</strong><br></p>");
        sb.Append("\n");
        
        sb.Append("Etiketler: ");
        sb.Append("kırmızı et,fiyat,sabitleme");
        return sb.ToString();
    }
    
    private string GetSubtitleTrainPrompt(string prompt)
    {
        var sb = new StringBuilder();
        sb.Append(prompt);
        sb.Append("\n");
        
        sb.Append("Ana Başlık: ");
        sb.Append("Antalya Büyükşehir Belediye Başkan adayları kim? 31 Mart yerel seçimlerinde Antalya Büyükşehir'den kaç kişi belediye başkan adayı oldu? Antalya Büyükşehir'de hangi parti kimi aday gösterdi?");
        sb.Append("\n");
        
        sb.Append("İçerik: ");
        sb.Append("<p><a title=\"31 Mart 2024 haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/31-mart-2024\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">31 Mart 2024</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">'te yapılacak olan&nbsp;</span><a title=\"yerel seçimler haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/yerel-secimler\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">yerel seçimler</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">de yarışacak olan adaylar belli oldu.</span><a title=\"Antalya haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/antalya\" style=\"background-color: rgb(255, 255, 255); color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; font-size: 19.2px; font-weight: 500;\">Antalya</a><span style=\"color: rgb(34, 34, 34); font-family: Montserrat, sans-serif; font-size: 19.2px;\">&nbsp;Büyükşehir Büyükşehir Belediye Başkanlığı için 22'si siyasi parti, 6'sı bağımsız toplam 28 aday seçime katılacak. Peki,</span><strong style=\"overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif; color: rgb(34, 34, 34); font-size: 19.2px;\">&nbsp;Antalya Büyükşehir Belediye Başkan adayları kim? 31 Mart yerel seçimlerinde Antalya'dan kaç kişi&nbsp;<a title=\"belediye başkan adayı haberleri\" target=\"_blank\" href=\"https://www.cumhuriyet.com.tr/haberleri/belediye-baskan-adayi\" style=\"color: rgb(161, 33, 33); text-decoration: none; overflow-wrap: break-word; word-break: break-word; font-family: Montserrat, sans-serif !important;\">belediye başkan adayı</a>&nbsp;oldu? Antalya'da hangi parti kimi aday gösterdi?</strong><br></p>");
        sb.Append("\n");
        
        sb.Append("Etiketler: ");
        sb.Append("mart,seçim,belediye,aday,antalya,parti,bağımsız,aday,seçim");
        return sb.ToString();
    }
}