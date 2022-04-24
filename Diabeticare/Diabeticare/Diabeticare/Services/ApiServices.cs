﻿using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Diabeticare.Services
{
    public class ApiServices
    {
        private readonly static HttpClient HttpClient;

        static ApiServices()
        {
            HttpClient = new HttpClient();

            // Might not be enough seconds
            HttpClient.Timeout = TimeSpan.FromSeconds(3);
        }

        private async Task<string> FetchToken()
        {
            /*
             * Sends a GET request to the server for a randomly generated CSRF token
             * Returns the CSRF token as a string
             * 
             * Return
             *  token: string
             *      A generated CSRF token
             */

            try
            {
                Uri get_uri = new Uri("http://10.0.2.2:8000/get_token");
                HttpResponseMessage get_response = await HttpClient.GetAsync(get_uri);
                string responseBody = await get_response.Content.ReadAsStringAsync();

                return (string)JObject.Parse(responseBody)["X-CSRFToken"];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task UpdateToken(HttpResponseMessage response)
        {
            /*
             * Reads the response body and updates the users token
             * 
             * Arguments
             *  Server response: HttpResponseMessage
             *      A response object containing the CSRF token
             */

            // Read token value
            string responseBody = await response.Content.ReadAsStringAsync();
            var token = (string)JObject.Parse(responseBody)["X-CSRFToken"];

            // Update token in database for user
            await App.Udatabase.UpdateUserTokenAsync(App.user, token);
        }

        private HttpRequestMessage createHttpRequestMessage(HttpMethod method, string url, string content, string token)
        {
            /*
             * Creates a HttpRequestMessage object
             * 
             * Arguments
             *  method: HttpMethod
             *      The request method (GET, POST, etc.)
             *  url: string
             *      The url to forward the request to
             *  content: string
             *      Message content
             *  token: string
             *      A valid CSRF token generated by the server
             *
             * Return
             *  Request Message: HttpRequestMessage
             *      A HttpRequestMessage populated with arguments
             */

            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-Version", "1" },
                    { "X-CSRFToken", $"{token}"},
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }
                },
                Content = new StringContent(content)
            };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string confirm)
        {
            /*
             * Sends a POST request to the server to register a new user
             * 
             * Arguments
             *  username: string
             *      Users name
             *  email: string
             *      Users email
             *  password: string
             *      Users password
             *  confirm: string
             *      Users password again
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             * 
             * Note
             * - Cannot send register request if user is already signed in
             */

            if (App.user != null)
                return false;

            try
            {
                string token = await FetchToken();

                var data = new
                {
                    username = username,
                    email = email,
                    password = password,
                    confirm = confirm
                };

                string url = "http://10.0.2.2:8000/u/register";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
                
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            /*
             * Sends a POST request to log in the user
             * On succesful authentication the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  email: string
             *      Users email
             *  password: string
             *      Users password
             *  confirm: string
             *      Users password again
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send login request if user is already signed in
             */

            if (App.user != null)
                return false;
            
            try
            {
                string token = await FetchToken();

                var data = new
                {
                    username = username,
                    password = password
                };

                string url = "http://10.0.2.2:8000/u/login";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                {
                    App.user = await App.Udatabase.GetUserEntryAsync(username);
                    await UpdateToken(response);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> LogoutAsync()
        {
            /*
             * Sends a POST request to log out the user
             * 
             * Note
             * - Cannot send logout request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = App.user.Username,
                };

                string url = "http://10.0.2.2:8000/u/logout";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> AddOrUpdateBGLAsync(string username, int value, int timestamp, string note = null, int identifier = -1)
        {
            /*
             * Sends a POST request to add or update bgl entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  value: int
             *      BGL value
             *  timestamp: int
             *      [Unix timestamp] Time user registered the value
             *  note: string (optional)
             *      A custom note for the entry
             *  identifier: int (optional)
             *      ID of existing entry for updating
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    value = value,
                    timestamp = timestamp,
                    note = note,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/bgl/set";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> FetchBGLAsync(string username, int timestamp)
        {
            /*
             * Sends a GET request to fetch all entries after timestamp
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  timestamp: int
             *      [Unix timestamp] Time user registered the value
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    timestamp = timestamp
                };

                string url = "http://10.0.2.2:8000/s/bgl/get";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Get, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteBGLAsync(string username, int identifier)
        {
            /*
             * Sends a POST request to delete specified entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  identifier: int
             *      ID to identify the entry (locally/server side)
             *      
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             * - Cannot delete entries that does not belong to the user
             * - Cannot delete entries that do not match the identifier
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/bgl/del";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> AddOrUpdateSleepAsync(string username, int start, int stop, string note = null, int identifier = -1)
        {
            /*
             * Sends a POST request to add or update sleep entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  start: int
             *      [Unix timestamp] Time user went to sleep
             *  stop: int
             *      [Unix timestamp] Time user woke up
             *  timestamp: int
             *       [Unix timestamp] Time user registered the value
             *  note: string (optional)
             *      A custom note for the entry
             *  identifier: int (optional)
             *      ID of existing entry for updating
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    start = start,
                    stop = stop,
                    note = note,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/sleep/set";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> FetchSleepAsync(string username, int timestamp)
        {
            /*
             * Sends a GET request to fetch all entries after timestamp
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  timestamp: int
             *      [Unix timestamp] Time user registered the value
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    timestamp = timestamp
                };

                string url = "http://10.0.2.2:8000/s/sleep/get";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Get, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteSleepAsync(string username, int identifier)
        {
            /*
             * Sends a POST request to delete specified entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  identifier: int
             *      ID to identify the entry (locally/server side)
             *      
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             * - Cannot delete entries that does not belong to the user
             * - Cannot delete entries that do not match the identifier
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/sleep/del";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> AddOrUpdateCIAsync(string username, int value, int timestamp, string note = null, int identifier = -1)
        {
            /*
             * Sends a POST request to add or update carbohydrate entry entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  value: int
             *      BGL value
             *  timestamp: int
             *      [Unix timestamp] Time user registered the value
             *  note: string (optional)
             *      A custom note for the entry
             *  identifier: int (optional)
             *      ID of existing entry for updating
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    value = value,
                    timestamp = timestamp,
                    note = note,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/ci/set";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> FetchCIAsync(string username, int timestamp)
        {
            /*
             * Sends a GET request to fetch all entries after timestamp
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  timestamp: int
             *      [Unix timestamp] Time user registered the value
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    timestamp = timestamp
                };

                string url = "http://10.0.2.2:8000/s/ci/get";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Get, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteCIAsync(string username, int identifier)
        {
            /*
             * Sends a POST request to delete specified entry
             * On succesful request the users token is updated
             * 
             * Arguments
             *  username: string
             *      Users name
             *  identifier: int
             *      ID to identify the entry (locally/server side)
             *      
             * 
             * Return
             *  Success: bool
             *      Whether the request succeeded or not
             *
             * Note
             * - Cannot send request if user is not logged in
             * - Cannot delete entries that does not belong to the user
             * - Cannot delete entries that do not match the identifier
             */

            if (App.user == null)
                return false;

            try
            {
                var data = new
                {
                    username = username,
                    identifier = identifier
                };

                string url = "http://10.0.2.2:8000/s/ci/del";
                string content = JsonConvert.SerializeObject(data);

                var httpRequestMessage = createHttpRequestMessage(HttpMethod.Post, url, content, App.user.Token);

                HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);

                // On success, update user and token
                if (response.IsSuccessStatusCode)
                    await UpdateToken(response);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }
    }
}
