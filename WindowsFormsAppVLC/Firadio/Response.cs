using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Firadio
{

    [DataContract]
    public class Response
    {
        [DataContract]
        public class DataContractData
        {
            [DataMember(Name = "rows")]
            public List<Dictionary<string, string>> Rows { get; internal set; }
        }
        [DataMember(Name = "table")]
        public DataContractData Table { get; internal set; }


    }
}
