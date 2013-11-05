using System;

namespace Retrofit.Net.Attributes
{
    public class ValueAttribute : Attribute
    {
        public string Value { get; protected set; }
    }
}