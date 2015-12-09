using System.Collections.Generic;
using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data
{
    public class Score : Notifieable
    {
        private string _score;
        private int _sort;
        private string _extraData;
        private string _username;
        private string _userId;
        private string _guest;
        private string _stored;

        [XmlElement("score")]
        public string ScoreText
        {
            get { return _score; }
            set { Set(ref _score, value); }
        }

        [XmlElement("sort")]
        public int Sort
        {
            get { return _sort; }
            set { Set(ref _sort, value); }
        }

        [XmlElement("extra_data")]
        public string ExtraData
        {
            get { return _extraData; }
            set { Set(ref _extraData, value); }
        }

        [XmlElement("user")]
        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        [XmlElement("user_id")]
        public string UserId
        {
            get { return _userId; }
            set { Set(ref _userId, value); }
        }

        [XmlElement("guest")]
        public string Guest
        {
            get { return _guest; }
            set { Set(ref _guest, value); }
        }

        [XmlElement("stored")]
        public string Stored
        {
            get { return _stored; }
            set { Set(ref _stored, value); }
        }
    }
    
    [XmlRoot("response")]
    public class ScoreResponse : SuccessResponse
    {
        private List<Score> _scores;

        [XmlArray("scores")]
        [XmlArrayItem("score", typeof(Score))]
        public List<Score> Scores
        {
            get { return _scores; }
            set { Set(ref _scores, value); }
        }
    }
}
