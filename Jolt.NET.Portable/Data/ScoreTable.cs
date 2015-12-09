using System.Collections.Generic;
using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data
{
    public class ScoreTable : Notifieable
    {
        private int _id;
        private string _name;
        private string _description;
        private bool _primary;

        [XmlElement("id")]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        [XmlElement("name")]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        [XmlElement("description")]
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        [XmlElement("primary")]
        public bool Primary
        {
            get { return _primary; }
            set { Set(ref _primary, value); }
        }
    }
    
    [XmlRoot("response")]
    public class ScoreTableResponse : SuccessResponse
    {
        private List<ScoreTable> _scoreTables;

        [XmlArray("tables")]
        [XmlArrayItem("table", typeof(ScoreTable))]
        public List<ScoreTable> Scores
        {
            get { return _scoreTables; }
            set { Set(ref _scoreTables, value); }
        }
    }
}
