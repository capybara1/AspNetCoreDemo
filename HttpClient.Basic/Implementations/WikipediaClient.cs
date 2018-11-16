using AspNetCoreDemo.HttpClient.Basic.Contracts;
using AspNetCoreDemo.HttpClient.Basic.Model;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCoreDemo.HttpClient.Basic
{
    public class WikipediaClient : IWikipediaClient
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        private readonly System.Net.Http.HttpClient _httpClient;

        public WikipediaClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<PageInfo> GetInfoAsync(string titles)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["action"] = "query";
            query["prop"] = "info";
            query["titles"] = titles;
            query["format"] = "json";
            var url = query.ToString();
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception();
            }

            var result = await DeserializeContentAsync<PageInfo>(response);
            return result;
        }

        private async Task<TType> DeserializeContentAsync<TType>(HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var textReader = new StreamReader(stream, new UTF8Encoding(false), false, 1024, true))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var result = _serializer.Deserialize<TType>(jsonReader);
                return result;
            }
        }
    }
}
