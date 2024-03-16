using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Firadio
{

    [DataContract]
    public class Response
    {

        [DataContract]
        public class Menu
        {
            [DataMember(Name = "name")]
            public string Name { get; internal set; }

            [DataMember(Name = "items")]
            public List<MenuItem> Items { get; internal set; }
        }

        [DataContract]
        public class MenuItem
        {
            [DataMember(Name = "name")]
            public string Name { get; internal set; }

            [DataMember(Name = "title")]
            public string Title { get; internal set; }

            [DataMember(Name = "rows")]
            public int Rows { get; internal set; }

            [DataMember(Name = "cols")]
            public int Cols { get; internal set; }

            [DataMember(Name = "start")]
            public int Start { get; internal set; }

            [DataMember(Name = "end")]
            public int End { get; internal set; }
        }

        [DataContract]
        public class Channel
        {
            [DataMember(Name = "title")]
            public string Title { get; internal set; }
            [DataMember(Name = "liveurl")]
            public string Liveurl { get; internal set; }
        }

        [DataMember(Name = "menus")]
        public List<Menu> Menus { get; internal set; }

        [DataMember(Name = "channels")]
        public List<Channel> Channels { get; internal set; }


    }
}
