using System;
using System.Runtime.Serialization;

namespace XPike.Configuration.Caching
{
    [Serializable]
    [DataContract]
    public class ConfigurationValue<TValue>
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public TValue Value { get; set; }
    }
}