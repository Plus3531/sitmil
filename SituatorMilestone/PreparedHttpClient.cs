using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SituatorMilestone
{
    public class PreparedHttpClient
    {
        private static HttpClient _httpclient ;
        private static string _sessionToken;
        private PreparedHttpClient()
        {
        }
        public static  Uri GetSituatorWebApiUrl()
        {
            return new Uri(ConfigurationManager.AppSettings["situatorWebApiUrl"], UriKind.Absolute);
        }
        private static async Task<string> LogIn(string user, string password)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(user, password);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-Version", "1");
            var sitUri = GetSituatorWebApiUrl();
            var url = sitUri + "/" + "api/Login";
            //string url = GetBaseAddress() + "/" + "api/Login";
            var response = await client.PostAsync(url, null);
            if (!response.IsSuccessStatusCode) return string.Empty;

            var responseCookies = cookies.GetCookies(sitUri).Cast<Cookie>();
            foreach (var cookie in responseCookies)
            {
                if (cookie.Name == "session-token")
                {
                    return cookie.Value;
                }
            }
            return string.Empty;
        }
        private static string GetBaseAddress()
        {
            var uri = new Uri(ConfigurationManager.AppSettings["situatorWebApiUrl"], UriKind.Absolute);
            //remove segments
            var noLastSegment = uri.GetComponents(UriComponents.SchemeAndServer,
                UriFormat.SafeUnescaped);
            return noLastSegment;
        }
        private static async Task<HttpClient> GetHttpClient()
        {
            var cookieContainer = new CookieContainer();
            //var token = textBoxToken.Text; //await Login();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            var result = new HttpClient(handler) { BaseAddress = new Uri(GetBaseAddress()) };
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (string.IsNullOrWhiteSpace(_sessionToken))
            {
                _sessionToken = await LogIn(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
            }
            cookieContainer.Add(result.BaseAddress, new Cookie("session-token", _sessionToken));

            return result;
        }
        public static async Task<HttpClient> GetInstance()
        {
            if (_httpclient == null)
            {
                _httpclient = await GetHttpClient();
            }
            return _httpclient;
        }
    }
}
