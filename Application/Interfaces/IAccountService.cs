using Application.DTOs.Users;
using Application.Wrappers;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<Response<AuthenticateResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
    }
}
