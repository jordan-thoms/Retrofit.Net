using RestSharp;

namespace Retrofit.Net.Attributes.Methods
{
    [RestMethod(Method.POST)]
    public class PostAttribute : ValueAttribute
    {
        public PostAttribute(string path)
        {
            this.Value = path;
        }
    }
}