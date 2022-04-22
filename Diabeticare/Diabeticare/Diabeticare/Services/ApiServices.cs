using System;
using System.Threading.Tasks;
using System.Net.Http;
using Diabeticare.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Diabeticare.Services
{
    public class ApiServices
    {
        private readonly static HttpClient HttpClient;
        private static string api_token;

        static ApiServices()
        {
            HttpClient = new HttpClient();
        }

        private async Task<string> FetchToken()
        {
            Uri get_uri = new Uri("http://10.0.2.2:8000/get_token");
            HttpResponseMessage get_response = await HttpClient.GetAsync(get_uri);
            string responseBody = await get_response.Content.ReadAsStringAsync();

            return (string) JObject.Parse(responseBody)["X-CSRFToken"];
        }

        private HttpRequestMessage createHttpRequestMessage(HttpMethod method, string url, string content, string api_token)
        {
            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-Version", "1" },
                    { "X-CSRFToken", $"{api_token}"},
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }
                },
                Content = new StringContent(content)
            };
        }

        public async Task<HttpResponseMessage> RegisterAsync(string username, string email, string password, string confirmPassword)
        {
            api_token = await FetchToken();

            var model = new RegisterBindingModel
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            string url = "http://10.0.2.2:8000/u/register";
            string content = JsonConvert.SerializeObject(model);

            var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, api_token);

            HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
            return response;
        }

        public async Task<HttpResponseMessage> LoginAsync(string username, string password)
        {
            api_token = await FetchToken();

            var model = new LoginBindingModel
            {
                Username = username,
                Password = password
            };

            string url = "http://10.0.2.2:8000/u/login";
            string content = JsonConvert.SerializeObject(model);

            var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, api_token);

            HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
            return response;
        }
    }
}
