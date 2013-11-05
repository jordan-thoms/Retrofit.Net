using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Retrofit.Net
{
    class RequestBuilder
    {
        private readonly RestMethodInfo methodInfo;
        private readonly object[] arguments;

        public RequestBuilder(RestMethodInfo methodInfo, object[] arguments)
        {
            this.methodInfo = methodInfo;
            this.arguments = arguments;
        }

        public IRestRequest Build()
        {
            var request =  new RestRequest(methodInfo.Path, methodInfo.Method);
            request.RequestFormat = DataFormat.Json; // TODO: Allow XML requests?
            for (int i = 0; i < arguments.Count(); i++)
            {
                Object argument = arguments[i];
                var usage = methodInfo.ParameterUsage[i];

                switch (usage)
                {
                    case RestMethodInfo.ParamUsage.Query:
                        request.AddParameter(methodInfo.ParameterNames[i], argument);
                        break;
                    case RestMethodInfo.ParamUsage.Path:
                        request.AddUrlSegment(methodInfo.ParameterNames[i], argument.ToString());
                        break;
                    case RestMethodInfo.ParamUsage.Body:
                        request.AddBody(argument);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return request;
        }
    }
}
