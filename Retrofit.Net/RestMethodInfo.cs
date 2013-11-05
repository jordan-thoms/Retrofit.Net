using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Retrofit.Net.Attributes;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Retrofit.Net
{
    class RestMethodInfo
    {
        private readonly MethodInfo methodInfo;
        protected object RequestMethod { get; set; }
        public RestSharp.Method Method { get; set; }
        public string Path { get; set; }

        public List<ParamUsage> ParameterUsage { get; set; }
        public List<string> ParameterNames { get; set; }

        internal enum ParamUsage
        {
            Query, Path, Body
        }

        public RestMethodInfo(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            Init(); // TODO: If supporting async, this should be deferred until needed so we don't
            // block the calling thread for longer than needed.
        }

        private void Init()
        {
            ParseMethodAttributes();
            ParseParameters();
        }

        private void ParseMethodAttributes()
        {
            foreach (ValueAttribute attribute in methodInfo.GetCustomAttributes(true))
            {
                var innerAttributes = attribute.GetType().GetCustomAttributes();

                // Find the request method attribute, if present.
                var methodAttribute = innerAttributes.FirstOrDefault(theAttribute => theAttribute.GetType() == typeof(RestMethodAttribute)) as RestMethodAttribute;

                if (methodAttribute != null)
                {
                    if (RequestMethod != null)
                    {
                        throw new ArgumentException("Method " + methodInfo.Name + " contains multiple HTTP methods. Found " + RequestMethod  + " and " + methodAttribute.Method);
                    }

                    Method = methodAttribute.Method;
                    Path = attribute.Value;
                }

                // TODO: Handle other types of annotations (e.g. headers) here
            }
        }

        private void ParseParameters()
        {
            ParameterUsage = new List<ParamUsage>();
            ParameterNames = new List<string>();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                var attribute = (ValueAttribute) parameter.GetCustomAttributes(false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new ArgumentException("No annotation found on parameter " + parameter.Name + " of " + methodInfo.Name);
                }
                var type = attribute.GetType();
                if (type == typeof(PathAttribute))
                {
                    ParameterUsage.Add(ParamUsage.Path);
                    ParameterNames.Add(attribute.Value);
                } else if (type == typeof (BodyAttribute))
                {
                    ParameterUsage.Add(ParamUsage.Body);
                    ParameterNames.Add(null);
                } else if (type == typeof(QueryAttribute))
                {
                    ParameterUsage.Add(ParamUsage.Query);
                    ParameterNames.Add(attribute.Value);
                }
            }
        }

    }
}
