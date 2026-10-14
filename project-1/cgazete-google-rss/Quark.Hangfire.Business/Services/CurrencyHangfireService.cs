using AvvaMobile.Core;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.SystemLog;
using Quark.Hangfire.Business.Interfaces;
using Quark.Hangfire.Business.Models.Responses.CurrenciesResponses;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Hangfire.Business.Services;

public class CurrencyHangfireService : BaseService, ICurrencyService
{
    private string nosyapiUrlTemplate = "{0}?apiKey={1}";
    private readonly ICommonCachingService _commonCachingService;

    public CurrencyHangfireService(QuarkDataContext context, IHttpContextAccessor httpContext,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache,
        ICommonCachingService commonCachingService) : base(
        httpContext, context, localizer, appSettingsKeys, cache)
    {
        _context = context;
        _commonCachingService = commonCachingService;
    }

    public async Task GetAllDailyCurrencyData()
    {
        var baseUrl = _appSettingsKeys.NosyAPI_CurrencyURL;
        var apiKey = _appSettingsKeys.NosyAPI_ApiKey;
        var networkManager = new NetworkManager();

        var hangfireLog = new HangfireLog()
        {
            CreatedDate = DateTime.Now,
            Method = (nameof(GetAllDailyCurrencyData)),
            Message = "Currency data retrieval started."
        };
        _context.HangfireLogs.Add(hangfireLog);
        await _context.SaveChangesAsync();

        try
        {
            var serviceUrl = string.Format(nosyapiUrlTemplate, baseUrl, apiKey);
            var networkResponse = await networkManager.GetAsync<NosyCurrencyResponse>(serviceUrl);

            if (!networkResponse.IsSuccess)
            {
                var hangfireErrorLog = new HangfireLog()
                {
                    CreatedDate = DateTime.Now,
                    Method = (nameof(GetAllDailyCurrencyData)),
                    Message = networkResponse.Message
                };
                _context.HangfireLogs.Add(hangfireErrorLog);
                await _context.SaveChangesAsync();
                return;
            }

            if (networkResponse.Data.Data.Count == 0)
            {
                var hangfireNotDataLog = new HangfireLog()
                {
                    CreatedDate = DateTime.Now,
                    Method = (nameof(GetAllDailyCurrencyData)),
                    Message = "No currency data found in the response."
                };
                _context.HangfireLogs.Add(hangfireNotDataLog);
                await _context.SaveChangesAsync();
                return;
            }

            var allCurrencies = await _context.Currencies.ToListAsync();

            foreach (var currency in allCurrencies)
            {
                var getCurrency = networkResponse.Data.Data.FirstOrDefault(x => x.Code == currency.Name);

                if (getCurrency == null)
                {
                    var hangfireNotCurrencyDataLog = new HangfireLog()
                    {
                        CreatedDate = DateTime.Now,
                        Method = (nameof(GetAllDailyCurrencyData)),
                        Message = $"Error: Currency not found in response. Currency Code: {currency.Code}"
                    };
                    _context.HangfireLogs.Add(hangfireNotCurrencyDataLog);
                    await _context.SaveChangesAsync();
                    continue;
                }

                var todayCurrency = await _context.CurrencyDailyValues.FirstOrDefaultAsync(x => x.Date.Date == DateTime.Now.Date
                    && x.CurrencyID == currency.ID);

                var lastDayCurrency = await _context.CurrencyDailyValues.FirstOrDefaultAsync(x =>
                    x.Date.Date == DateTime.Now.AddDays(-1).Date
                    && x.CurrencyID == currency.ID);

                if (todayCurrency == null)
                {
                    todayCurrency = new CurrencyDailyValue();
                    todayCurrency.CurrencyID = currency.ID;
                    todayCurrency.Date = DateTime.Now;
                    todayCurrency.BuyingValue = Math.Round(getCurrency.Buying, 2);
                    todayCurrency.SellingValue = Math.Round(getCurrency.Selling, 2);
                    todayCurrency.StateID = lastDayCurrency?.SellingValue == Math.Round(getCurrency.Selling, 2)
                        ? (int)CurrencyStateEnum.Equal
                        : (lastDayCurrency?.SellingValue < Math.Round(getCurrency.Selling, 2)
                            ? (int)CurrencyStateEnum.Increased
                            : (int)CurrencyStateEnum.Decreased);

                    _context.CurrencyDailyValues.Add(todayCurrency);
                }
                else
                {
                    todayCurrency.BuyingValue = Math.Round(getCurrency.Buying, 2);
                    todayCurrency.SellingValue = Math.Round(getCurrency.Selling, 2);
                    todayCurrency.Date = DateTime.Now;

                    _context.CurrencyDailyValues.Update(todayCurrency);
                }

                await _commonCachingService.RefreshCurrencyDailyData(currency.Slug);
            }

            var finishHangfireLog = new HangfireLog()
            {
                CreatedDate = DateTime.Now,
                Method = (nameof(GetAllDailyCurrencyData)),
                Message = "Currency data retrieval completed successfully."
            };

            _context.HangfireLogs.Add(finishHangfireLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var hangfireErrorLog = new HangfireLog()
            {
                CreatedDate = DateTime.Now,
                Method = (nameof(GetAllDailyCurrencyData)),
                Message = $"Error: {e.Message}"
            };
            _context.HangfireLogs.Add(hangfireErrorLog);
        }
    }
}