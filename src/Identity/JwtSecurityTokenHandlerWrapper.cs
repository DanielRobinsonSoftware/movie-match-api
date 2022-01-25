using System.IdentityModel.Tokens.Jwt;

namespace MovieMatch.Identity
{
    /// <summary>
    /// System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler does not implement an interface. 
    /// This class allows substition of JwtSecurityTokenHandler in unit tests.
    /// </summary>
    public class JwtSecurityTokenHandlerWrapper : JwtSecurityTokenHandler, IJwtSecurityTokenHandler
    {
        public JwtSecurityTokenHandlerWrapper() : base()
        {
        }
    }
}