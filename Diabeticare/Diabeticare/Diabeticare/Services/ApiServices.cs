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
        private static readonly HttpClient HttpClient;

        static ApiServices()
        {
            HttpClient = new HttpClient();
        }

        public async Task RegisterAsync(string username, string email, string password, string confirmPassword)
        {
            // Make own function eventually
            Uri get_uri = new Uri("http://10.0.2.2:8000/get_token");
            HttpResponseMessage get_response = await HttpClient.GetAsync(get_uri);
            get_response.EnsureSuccessStatusCode();
            string responseBody = await get_response.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(responseBody);
            var token = json["CSRF-Token"];
            // END

            var model = new RegisterBindingModel
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://10.0.2.2:8000/u/register"),
                Headers =
                {
                    { "X-Version", "1" },
                    { "X-CSRFToken", $"{token}"},
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }
                },
                Content = new StringContent(JsonConvert.SerializeObject(model))
            };

            var response = await HttpClient.SendAsync(httpRequestMessage);
        }

        private class jsonresponse
        {
            string token { get; set; }
        };
    }
}
