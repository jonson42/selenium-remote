using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ClientResponse<T>
    {
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ClientResponse()
        {
            StatusCode = HttpStatusCode.OK;
        }
        public string Message { get; set; }
    }

    public abstract class ThirdPartyClientBase
    {
        protected HttpClient Client { get; set; }
        public string _token { get; set; }
        private readonly string HTTP_CLIENT_REQUEST_TIMEOUT = Environment.GetEnvironmentVariable("HTTP_CLIENT_REQUEST_TIMEOUT");

        public virtual async Task<ClientResponse<T>> GetAsync<T>(string path, NameValueCollection request = null)
        {
            ClientResponse<T> result = new ClientResponse<T>();
            try
            {
                HttpResponseMessage response = await Client.GetAsync(new Uri(Client.BaseAddress.ToString() + path)).ConfigureAwait(false);
                string jsonString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    result.StatusCode = response.StatusCode;
                }

                if (!string.IsNullOrEmpty(jsonString))
                {
                    result.Data = JsonConvert.DeserializeObject<T>(jsonString);
                }
                else
                {
                    result.StatusCode = HttpStatusCode.NoContent;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }
                result.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            catch (Exception ex)
            {
                result.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            return result;
        }

        public virtual async Task<ClientResponse<T>> PostAsync<T>(string path, object request)
        {
            ClientResponse<T> result = new ClientResponse<T>();
            try
            {
                string content = JsonConvert.SerializeObject(request);
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                ByteArrayContent byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await Client.PostAsync(string.Format("{0}{1}", Client.BaseAddress, path), byteContent).ConfigureAwait(false);
                string jsonString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    result.StatusCode = response.StatusCode;
                }

                if (!string.IsNullOrEmpty(jsonString))
                {
                    result.Data = JsonConvert.DeserializeObject<T>(jsonString);
                }
                else
                {
                    result.StatusCode = HttpStatusCode.NoContent;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }
                result.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            catch (Exception ex)
            {
                result.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            return result;
        }

        public void SetTimeOut(int timeout = 0)
        {
            if (timeout == 0)
            {
                Int32.TryParse(HTTP_CLIENT_REQUEST_TIMEOUT, out timeout);
                if (timeout == 0)
                {
                    timeout = 30;
                }
            }
            Client.Timeout = TimeSpan.FromSeconds(timeout);
        }
    }
}
