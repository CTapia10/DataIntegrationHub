using DataIntegrationHub.Entities;

namespace DataIntegrationHub.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
