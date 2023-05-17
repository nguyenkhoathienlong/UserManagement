namespace Data.Models
{
    public class DbSetupModel
    {
        public string ConnectionStrings { get; set; } = string.Empty;
    }

    public class JwtModel
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public string? Secret { get; set; }
    }

    public class JWTToken
    {
        public string? TokenString { get; set; }
        public long ExpiresInMilliseconds { get; set; }
    }
}
