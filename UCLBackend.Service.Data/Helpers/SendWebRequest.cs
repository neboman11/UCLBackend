using System.Threading;
using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UCLBackend.Service.Data.Helpers
{
    public static class SendWebRequest
    {
        public static async Task<T> GetAsync<T>(Uri uri, string authorizationHeaderValue)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.GetAsync(uri.ToString());

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.GetAsync(uri.ToString());
                }

                await ValidateResponseCode(response);

                T result = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);

                response.Dispose();

                return result;
            }
        }

        public static async Task<byte[]> GetDataAsync(Uri uri, string authorizationHeaderValue)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.GetAsync(uri.ToString());

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.GetAsync(uri.ToString());
                }

                await ValidateResponseCode(response);

                var result = await response.Content.ReadAsByteArrayAsync();

                response.Dispose();

                return result;
            }
        }

        public static async Task PostAsync(Uri uri, string authorizationHeaderValue, HttpContent content)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.PostAsync(uri.ToString(), content);

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.PostAsync(uri.ToString(), content);
                }

                await ValidateResponseCode(response);

                response.Dispose();
            }
        }

        public static async Task<T> PostAsync<T>(Uri uri, string authorizationHeaderValue, HttpContent content)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.PostAsync(uri.ToString(), content);

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.PostAsync(uri.ToString(), content);
                }

                await ValidateResponseCode(response);

                T result = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);

                response.Dispose();

                return result;
            }
        }

        public static async Task PatchAsync(Uri uri, string authorizationHeaderValue, HttpContent content)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.PatchAsync(uri.ToString(), content);

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.PatchAsync(uri.ToString(), content);
                }

                await ValidateResponseCode(response);

                response.Dispose();
            }
        }

        public static async Task PutAsync(Uri uri, string authorizationHeaderValue, HttpContent content)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.PutAsync(uri.ToString(), content);

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.PutAsync(uri.ToString(), content);
                }

                await ValidateResponseCode(response);

                response.Dispose();
            }
        }

        public static async Task DeleteAsync(Uri uri, string authorizationHeaderValue)
        {
            // Create a new HTTP client
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authorizationHeaderValue))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                }

                // Send the request
                var response = await client.DeleteAsync(uri.ToString());

                while (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(5000);
                    response.Dispose();
                    response = await client.DeleteAsync(uri.ToString());
                }

                await ValidateResponseCode(response);

                response.Dispose();
            }
        }

        // TODO: Find way to error messages from BaseResponse on internal errors
        private static async Task ValidateResponseCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode} {response.ReasonPhrase}: {responseMessage}");
            }
        }
    }
}