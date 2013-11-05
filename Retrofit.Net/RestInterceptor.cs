using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit.Net
{
    public class RestInterceptor : IInterceptor
    {
        private IRestClient restClient;

        public RestInterceptor(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public void Intercept(IInvocation invocation)
        {
            // Build Request
            var methodInfo = new RestMethodInfo(invocation.Method); // TODO: Memoize these objects in a hash for performance
            var request = new RequestBuilder(methodInfo, invocation.Arguments).Build();

            // Execute request
            var responseType = invocation.Method.ReturnType;
            var genericTypeArgument = responseType.GenericTypeArguments[0];
            // We have to find the method manually due to limitations of GetMethod()
            var methods = restClient.GetType().GetMethods();
            MethodInfo method = methods.Where(m => m.Name == "Execute").First(m => m.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(genericTypeArgument);
            invocation.ReturnValue =  generic.Invoke(restClient, new object[] { request });

        }
    }
}