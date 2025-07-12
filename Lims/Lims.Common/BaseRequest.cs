using RestSharp;

namespace Lims.Common
{
    public class BaseRequest
    {
        public Method Method { get; set; }
        public string Route { get; set; }
        public string ContentType { get; set; } = "application/json;charset=utf-8";
        public object Parameter { get; set; }
    }
}
