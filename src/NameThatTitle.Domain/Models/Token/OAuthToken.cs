using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NameThatTitle.Domain.Models.Token
{
    public class OAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "bearer";

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
    }
}
