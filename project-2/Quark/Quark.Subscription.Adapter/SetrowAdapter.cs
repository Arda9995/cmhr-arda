using System.Text;
using System.Text.Json;
using AvvaMobile.Core.Business;
using Quark.Subscription.Adapter.Constants.Setrow;
using Quark.Subscription.Adapter.Models.Requests.Setrow;

namespace Quark.Subscription.Adapter;

public class SetrowAdapter(HttpClient httpClient)
{
    public async Task<ServiceResult<string>> CreateSetrowSubscription(
        CreateSetrowSubscriptionRequest subscriptionRequest)
    {
        var result = new ServiceResult<string>();

        try
        {
            string url = $"{subscriptionRequest.BaseUrl}/{Endpoints.AddSetrowSubscription}";

            var payload = new[]
            {
                new SetrowEmailRequest
                {
                    email = subscriptionRequest.Email,
                    name = subscriptionRequest.Name,
                    groupId = subscriptionRequest.GroupID.ToString(),
                    iys = new IysInfo
                    {
                        emailAllowStatus = "Y",
                        emailOptInSource = 3,
                        emailOptInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                }
            };

            string json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", subscriptionRequest.ApiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.IsSuccess = true;
                result.Data = responseString;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = $"Error: {response.StatusCode}, Details: {responseString}";
            }
        }
        catch (Exception e)
        {
            result.IsSuccess = false;
            result.Message = e.Message;
            return result;
        }

        return result;
    }
}