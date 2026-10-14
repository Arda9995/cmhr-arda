using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Quark.CMS.Business.Models.Requests.NewsRequests;

var services = new ServiceCollection();
services.AddLogging();
services.AddMvcCore();
using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var metadataProvider = scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>();
var metadata = metadataProvider.GetMetadataForType(typeof(NewsRequest));
var binder = scope.ServiceProvider.GetRequiredService<IModelBinderFactory>().CreateBinder(
    new ModelBinderFactoryContext { Metadata = metadata, CacheToken = typeof(NewsRequest) });

async Task CheckBinding(string name, Dictionary<string, StringValues> values, bool expectedAllow)
{
    var http = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
    http.Request.ContentType = "application/x-www-form-urlencoded";
    http.Request.Form = new FormCollection(values);
    var action = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
    var binding = DefaultModelBindingContext.CreateBindingContext(action,
        new FormValueProvider(BindingSource.Form, http.Request.Form, CultureInfo.InvariantCulture),
        metadata, null, string.Empty);
    await binder.BindModelAsync(binding);
    if (!binding.Result.IsModelSet || action.ModelState.ErrorCount != 0 ||
        binding.Result.Model is not NewsRequest request ||
        request.AllowGoogleAiFeed != expectedAllow || request.ExcludeFromGoogleAiFeed == expectedAllow)
        throw new InvalidOperationException($"FAIL: {name}; model set={binding.Result.IsModelSet}, binding errors={action.ModelState.ErrorCount}, allow={(binding.Result.Model as NewsRequest)?.AllowGoogleAiFeed}, exclude={(binding.Result.Model as NewsRequest)?.ExcludeFromGoogleAiFeed}");
    Console.WriteLine($"PASS: {name}");
}

await CheckBinding("ON plus Razor hidden false value allows the article", new()
{
    [nameof(NewsRequest.AllowGoogleAiFeed)] = new StringValues(new[] { "true", "false" })
}, true);
await CheckBinding("OFF posts false and excludes the article", new()
{
    [nameof(NewsRequest.AllowGoogleAiFeed)] = "false"
}, false);
await CheckBinding("Missing switch preserves the existing excluded-by-default behavior", new(), false);
await CheckBinding("Legacy exclusion=true posts still exclude", new()
{
    [nameof(NewsRequest.ExcludeFromGoogleAiFeed)] = "true"
}, false);
await CheckBinding("Legacy exclusion=false posts still allow", new()
{
    [nameof(NewsRequest.ExcludeFromGoogleAiFeed)] = "false"
}, true);

var edited = new NewsRequest { ExcludeFromGoogleAiFeed = false };
if (!edited.AllowGoogleAiFeed)
    throw new InvalidOperationException("An included article must display ON when edited.");
edited.ExcludeFromGoogleAiFeed = true;
if (edited.AllowGoogleAiFeed)
    throw new InvalidOperationException("An excluded article must display OFF when edited.");
edited.AllowGoogleAiFeed = true;
if (edited.ExcludeFromGoogleAiFeed)
    throw new InvalidOperationException("Turning ON must clear the stored exclusion flag.");
edited.AllowGoogleAiFeed = false;
if (!edited.ExcludeFromGoogleAiFeed)
    throw new InvalidOperationException("Turning OFF must set the stored exclusion flag.");
Console.WriteLine("PASS: edit display and ON/OFF transitions preserve inverse mapping.");
Console.WriteLine("All Google AI switch checks passed. No database or external services were accessed.");
