using Newtonsoft.Json;

namespace TutorService.Application.Common;

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonProperty("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; init; }
}
