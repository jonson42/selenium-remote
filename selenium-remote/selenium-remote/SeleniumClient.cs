using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace selenium_remote
{
    public interface ISeleniumClient
    {
        Task<ClientResponse<T>> GetAsync<T>(string path, NameValueCollection request = null);
        Task<ClientResponse<T>> PostAsync<T>(string path, object request);
    }
    public class SeleniumClient:ThirdPartyClientBase
    {
        public SeleniumClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var _client = new HttpClient(clientHandler);
            string apiKey = ConfigurationManager.AppSettings["SELENIUM_API_KEY"];
            string clmsApiKey = ConfigurationManager.AppSettings["SELENIUM_API_KEY"];
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SELENIUM_API_KEY"]);
            _client.DefaultRequestHeaders.Add("x-apikey", clmsApiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("UTF-8"));
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(System.Reflection.Assembly.GetEntryAssembly().GetName().Name)));
            Client = _client;
            SetTimeOut();
        }
    }
}
