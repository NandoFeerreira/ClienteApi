namespace ClienteApi.Application.Common.Constants
{
    public static class ErrorMessages
    {
        public const string CepsDuplicados = "CEPs duplicados encontrados na requisição: {0}";
        public const string CepNaoEncontrado = "CEP '{0}' não encontrado";
        public const string TiposContatoInvalidos = "Tipos de contato inválidos: {0}. Tipos válidos: {1}";
        public const string ContatosDuplicados = "Contatos duplicados encontrados na requisição: {0}";
        public const string ClienteNaoEncontrado = "Cliente com ID {0} não encontrado";
        public const string EnderecoNaoEncontrado = "Endereço com ID {0} não encontrado para este cliente";
        public const string ContatoNaoEncontrado = "Contato com ID {0} não encontrado para este cliente";
    }
}
