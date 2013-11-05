using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit.Net
{
    public class RestAdapter
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private IRestClient restClient;

        public RestAdapter(string baseUrl)
        {
            this.restClient = new RestClient(baseUrl);
        }

        public RestAdapter(IRestClient client)
        {
            this.restClient = client;
        }



        public string Server
        {
            get; set; }

        public T Create<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>(new RestInterceptor(restClient));
        }
    }
}