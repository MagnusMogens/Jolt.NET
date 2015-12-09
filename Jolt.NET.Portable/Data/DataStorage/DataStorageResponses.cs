using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data.DataStorage
{
    [XmlRoot("response")]
    public class GetKeysResponse : SuccessResponse
    {
        private List<DataStorageKey> _keys;

        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public List<DataStorageKey> Keys
        {
            get { return _keys; }
            set { Set(ref _keys, value); }
        }
    }
    
    [XmlRoot("response")]
    public class UpdateDataStorageResponse : SuccessResponse
    {
        private object _data;

        [XmlElement("data")]
        public object Data
        {
            get { return _data; }
            set { Set(ref _data, value); }
        }
    }

    /// <summary>
    /// I need an extra class which capsules the key entry, because the xml response contains a key element in 
    /// a key element.
    /// </summary>
    [XmlRoot("key")]
    public class DataStorageKey : Notifieable
    {
        private string _key;

        [XmlElement("key")]
        public string Key
        {
            get { return _key; }
            set { Set(ref _key, value); }
        }
    }
}
