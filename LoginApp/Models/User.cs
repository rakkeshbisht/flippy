using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace LoginAppService.Models
{
    public class User
    {
        public ObjectId _id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string clientid { get; set; }
        public string scope { get; set; }
        public string clientsecret { get; set; }

    }

    public class UserToken
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}