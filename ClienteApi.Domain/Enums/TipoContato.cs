namespace ClienteApi.Domain.Enums
{

    public static class TipoContato
    {
        public const string Email = "Email";
        public const string Celular = "Celular";
        public const string Telefone = "Telefone";
        public const string WhatsApp = "WhatsApp";

      
        public static readonly string[] TiposValidos =
        [
            Email,
            Celular,
            Telefone,
            WhatsApp
        ];

    
        public static bool EhValido(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return false;

            return TiposValidos.Any(t => t.Equals(tipo.Trim(), StringComparison.OrdinalIgnoreCase));
        }

     
        public static string? Normalizar(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return null;

            var tipoTrimmed = tipo.Trim();
            return TiposValidos.FirstOrDefault(t => t.Equals(tipoTrimmed, StringComparison.OrdinalIgnoreCase));
        }
    }
}
