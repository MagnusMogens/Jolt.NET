using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Jolt.NET.Base;

namespace Jolt.NET.Data
{
    public class User : Notifieable
    {
        private int _id;
        private UserType _type;
        private string _username;
        private string _avatarUrl;
        private string _signedUp;
        private string _lastLoggedIn;
        private UserStatus _status;
        private string _developerName;
        private string _developerWebsite;
        private string _developerDescription;
        private bool _isAuthenticated;
        private string _token;
        
        [XmlElement("id")]
        public int Id 
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        
        [XmlElement("type")]
        public UserType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }
        
        [XmlElement("username")]
        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }
        
        [XmlElement("avatar_url")]
        public string AvatarUrl
        {
            get { return _avatarUrl; }
            set { Set(ref _avatarUrl, value); }
        }
        
        [XmlElement("signed_up")]
        public string SignedUp
        {
            get { return _signedUp; }
            set { Set(ref _signedUp, value); }
        }
        
        [XmlElement("last_logged_in")]
        public string LastLoggedIn 
        {
            get { return _lastLoggedIn; }
            set { Set(ref _lastLoggedIn, value); }
        }
        
        [XmlElement("status")]
        public UserStatus Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }
        
        [XmlElement("developer_name")]
        public string DeveloperName
        {
            get { return _developerName; }
            set { Set(ref _developerName, value); }
        }
        
        [XmlElement("developer_website")]
        public string DeveloperWebsite
        {
            get { return _developerWebsite; }
            set { Set(ref _developerWebsite, value); }
        }
        
        [XmlElement("developer_description")]
        public string DeveloperDescription
        {
            get { return _developerDescription; }
            set { Set(ref _developerDescription, value); }
        }
        
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set { Set(ref _isAuthenticated, value); }
        }
        
        public string Token
        {
            get { return _token; }
            set { Set(ref _token, value); }
        }

        #region Overriden methods

        /// <summary>
        /// Determines whether the given objects Username & Token is equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var user = (obj as User);
            if (user == null)
                return false;

            return base.Equals(obj) ||
                   (user.Username == Username &&
                    user.Token == user.Token);
        }

        /// <summary>
        /// Determines whether the given objects Username & Token is equal.
        /// </summary>
        public static bool operator ==(User a, User b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
                return false;

            // Return true if the username and token matches.
            return a.Username.Equals(b.Username, StringComparison.OrdinalIgnoreCase) &&
                   a.Token.Equals(b.Token, StringComparison.OrdinalIgnoreCase); 
        }

        /// <summary>
        /// Determines whether the given objects Username & Token is not equal.
        /// </summary>
        public static bool operator !=(User a, User b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return false;

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
                return true;

            // Return true if the username and token matches.
            return a.Username != b.Username ||
                   a.Token != b.Token; 
        }

        /// <summary>
        /// Returns a hash code for this instance. The hash is calculated by the username and token.
        /// </summary>
        public override int GetHashCode()
        {
            return (Username + Token).GetHashCode();
        }

        #endregion
    }
    
    [XmlRoot("response")]
    public class UserResponse : SuccessResponse
    {
        private List<User> _users;

        [XmlArray("users")]
        [XmlArrayItem("user", typeof(User))]
        public List<User> Users
        {
            get { return _users; }
            set { Set(ref _users, value); }
        }
    }
}