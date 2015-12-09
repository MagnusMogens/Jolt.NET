using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Jolt.NET.Core;

namespace Jolt.NET.Data
{
    public class StoredData : Notifieable
    {
        private bool? _success;
        private string _data;
    }

    [Serializable()]
    [XmlRoot("key")]
    public class StoredDataKey : Notifieable
    {
        private string _key;

        [XmlElement("key")]
        public string Key
        {
            get { return _key; }
            set { Set(ref _key, value); }
        }
    }

    [Serializable()]
    [XmlRoot("response")]
    public class StoredDataKeyResponse : SuccessResponse
    {
        private List<StoredDataKey> _keys;

        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public List<StoredDataKey> Keys
        {
            get { return _keys; }
            set { Set(ref _keys, value); }
        }
    }
}
