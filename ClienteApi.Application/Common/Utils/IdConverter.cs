
using System;

namespace ClienteApi.Application.Common.Utils
{
    public static class IdConverter
    {
        
        public static string ToBase64(int id)
        {
            var bytes = BitConverter.GetBytes(id);
            var base64 = Convert.ToBase64String(bytes);
            
            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

       
        public static int FromBase64(string base64Id)
        {
            if (string.IsNullOrWhiteSpace(base64Id))
            {
                throw new ArgumentException("O ID em Base64 não pode ser nulo ou vazio.", nameof(base64Id));
            }

           
            var base64 = base64Id.Replace('-', '+').Replace('_', '/');            
            
            base64 += (base64.Length % 4) switch
            {
                2 => "==",
                3 => "=",
                _ => ""
            };

            try
            {
                var bytes = Convert.FromBase64String(base64);

                if (bytes.Length != sizeof(int))
                {
                    throw new FormatException($"O ID fornecido não tem o tamanho esperado. Esperado: {sizeof(int)} bytes, Recebido: {bytes.Length} bytes.");
                }

                return BitConverter.ToInt32(bytes, 0);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException("O formato do ID fornecido é inválido.", ex);
            }
        }
    }
}
