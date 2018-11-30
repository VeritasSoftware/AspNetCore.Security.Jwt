namespace AspNetCore.Security.Jwt.UnitTests
{
    class UserModel : IAuthenticationUser
    {
        public string Id { get; set; }
        public string Pwd { get; set; }
        public string Role { get; set; }
        public string DOB { get; set; }
    }
}
