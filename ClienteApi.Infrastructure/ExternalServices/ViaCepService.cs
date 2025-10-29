using ClienteApi.Application.DTOs.ViaCep;
using ClienteApi.Application.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClienteApi.Infrastructure.ExternalServices
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ViaCepResponse?> GetAddressByCepAsync(string cep)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cep))
                    return null;

                var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());

                if (cepLimpo.Length != 8)
                    return null;

                var url = $"{cepLimpo}/json/";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao consultar ViaCEP para o CEP {CEP}. StatusCode: {StatusCode}", cep, response.StatusCode);
                    return null;
                }

                var viaCepResponse = await response.Content.ReadFromJsonAsync<ViaCepResponse>(_jsonOptions);

                if (viaCepResponse?.Erro == true)
                {
                    _logger.LogInformation("ViaCEP retornou erro para o CEP {CEP}. O CEP pode ser inválido.", cep);
                    return null;
                }

                return viaCepResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de HTTP ao consultar ViaCEP para o CEP {CEP}", cep);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro de JSON ao processar resposta do ViaCEP para o CEP {CEP}", cep);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no ViaCepService ao consultar o CEP {CEP}", cep);
                return null;
            }
        }
    }
}
