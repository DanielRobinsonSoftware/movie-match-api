using System.Threading.Tasks;

namespace MovieMatch.Identity
{
    public interface IAzureADJwtBearerValidation
    {
        Task<bool> ValidateTokenAsync(string authorizationHeader);
    }
}