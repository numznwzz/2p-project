using Domain.Transport;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Transport
{
    public class RestAPI : IRestAPI
    {

        public async Task<string> Post(string url, string version, string path, Object obj)
        {
            try
            {
                var client = new RestClient(url);
                
                var request = new RestRequest(version+"/"+path);

                request.Method = Method.Post;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("X-Correlation-ID", Guid.NewGuid().ToString(), ParameterType.HttpHeader);
                string jsonString = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(jsonString);
             //   request.Timeout = 1000*5;

                var response = await client.ExecutePostAsync(request);

                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<string> Get(string url,string version,string path,Dictionary<string,string> param)
        {
            try
            {
                var client = new RestClient(url);

                RestRequest request = null;
                
                if(version!=null)
                    request = new RestRequest("/"+version+"/"+path);
                else
                {
                    request = new RestRequest("/"+path);
                }

                request.Method = Method.Get;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("X-Correlation-ID",Guid.NewGuid().ToString());
                request.Timeout = 1000*30;

                foreach (var value in param)
                {
                    request.AddParameter(value.Key, value.Value);
                }
            
                request.AddParameter("X-Correlation-ID", Guid.NewGuid().ToString(), ParameterType.HttpHeader);

                var response = await client.ExecuteGetAsync(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e.ToString());
            }

            return null;
        }

        public void Dispose()
        {
            
        }
    }
}