using Lims.Common;
using Newtonsoft.Json;
using RestSharp;

namespace Lims.WPF.Services
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;

        public HttpRestClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
            client = new RestClient();          
        }

        public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            try
            {
                var request = new RestRequest(new Uri(apiUrl + baseRequest.Route), baseRequest.Method);
                request.AddHeader("Content-Type", baseRequest.ContentType);
                //request.AddXmlBody("Content-Type", baseRequest.ContentType);
                if (baseRequest.Parameter != null)
                    request.AddParameter("param", JsonConvert.SerializeObject(baseRequest.Parameter), ParameterType.RequestBody);
                //request.Body.ContentType = baseRequest.ContentType;

                IRestResponse response = await client.ExecuteAsync(request);

                  if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    var content = response.Content;
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content, settings);
                }
                else
                    return new ApiResponse<T>()
                    {
                        Status = false,
                        Message = response.ErrorMessage
                    };
            }
            catch (Exception e)
            {
               
                throw new Exception(e.Message);
            }

        }
    }


}
