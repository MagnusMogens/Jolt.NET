using System.Collections.Generic;
using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data
{
    public class Trophy : Notifieable
    {
        private int _id;
        private string _title;
        private string _description;
        private TrophyDifficulty _difficulty;
        private string _imageUrl;
        private string _achieved;
        
        [XmlElement("id")]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        
        [XmlElement("title")]
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }
        
        [XmlElement("description")]
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }
        
        [XmlElement("difficulty")]
        public TrophyDifficulty Difficulty
        {
            get { return _difficulty; }
            set { Set(ref _difficulty, value); }
        }
        
        [XmlElement("image_url")]
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { Set(ref _imageUrl, value); }
        }
        
        [XmlElement("achieved")]
        public string Achieved
        {
            get { return _achieved; }
            set { Set(ref _achieved, value); }
        }
    }
    
    [XmlRoot("response")]
    public class TrophyResponse : SuccessResponse
    {
        private List<Trophy> _trophies;
        
        [XmlArray("trophies")]
        [XmlArrayItem("trophy", typeof(Trophy))]
        public List<Trophy> Trophies
        {
            get { return _trophies; }
            set { Set(ref _trophies, value); }
        }
    }
}
