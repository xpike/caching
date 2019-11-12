using ProtoBuf;
using System;
using System.Runtime.Serialization;

namespace Example.Library
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class RandomItem
    {
        [DataMember]
        [ProtoMember(1)]
        public DateTime Timestamp { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string Something { get; set; }
    }
}