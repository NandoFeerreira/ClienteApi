using ClienteApi.Application.DTOs.ViaCep;

namespace ClienteApi.Application.Services
{
    public interface IViaCepService
    {
        Task<ViaCepResponse?> GetAddressByCepAsync(string cep);
    }
}
