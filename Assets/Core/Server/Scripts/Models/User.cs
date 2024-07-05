using JWT.Builder;

namespace MMC.Server.Models
{
    public class UserModel : Model
    {
        public string username;

        public void BuildToken(JwtBuilder builder)
        {
            builder.AddClaim("id", id.ToString());
        }
    }
}