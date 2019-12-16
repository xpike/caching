using System;
using System.Runtime.Serialization;

namespace Example.Library
{
    [Serializable]
    [DataContract]
    public class TestConfig
    {
        [DataMember]
        public string TestValue { get; set; }

        [DataMember]
        public DateTime Loaded { get; set; }

        public TestConfig()
        {
            Loaded = DateTime.UtcNow;
        }
    }
}