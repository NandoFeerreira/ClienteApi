using ClienteApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Infrastructure.Data
{
 
    public static class DatabaseSeeder
    {
       
        public static void SeedData(ApplicationDbContext context)
        {
            
            context.Database.EnsureCreated();
       
            if (context.Clientes.Any())
            {
                return; 
            }
          
            var clientesExemplo = new List<Cliente>
            {
                new Cliente
                {
                    Nome = "João Silva",
                    DataCadastro = DateTime.Now.AddDays(-30),
                    Enderecos = new List<Endereco>
                    {
                        new Endereco
                        {
                            Cep = "01001000",
                            Logradouro = "Praça da Sé",
                            Numero = "100",
                            Complemento = "Apto 101",
                            Cidade = "São Paulo"
                        }
                    },
                    Contatos = new List<Contato>
                    {
                        new Contato
                        {
                            Tipo = "Telefone",
                            Texto = "(11) 98765-4321"
                        },
                        new Contato
                        {
                            Tipo = "Email",
                            Texto = "joao.silva@email.com"
                        }
                    }
                },
                new Cliente
                {
                    Nome = "Maria Santos",
                    DataCadastro = DateTime.Now.AddDays(-15),
                    Enderecos = new List<Endereco>
                    {
                        new Endereco
                        {
                            Cep = "20040020",
                            Logradouro = "Avenida Rio Branco",
                            Numero = "200",
                            Complemento = "Sala 501",
                            Cidade = "Rio de Janeiro"
                        }
                    },
                    Contatos = new List<Contato>
                    {
                        new Contato
                        {
                            Tipo = "Celular",
                            Texto = "(21) 99999-8888"
                        },
                        new Contato
                        {
                            Tipo = "Email",
                            Texto = "maria.santos@empresa.com"
                        }
                    }
                },
                new Cliente
                {
                    Nome = "TechCorp Sistemas",
                    DataCadastro = DateTime.Now.AddDays(-60),
                    Enderecos = new List<Endereco>
                    {
                        new Endereco
                        {
                            Cep = "30130100",
                            Logradouro = "Avenida Afonso Pena",
                            Numero = "1500",
                            Complemento = "Andar 15",
                            Cidade = "Belo Horizonte"
                        }
                    },
                    Contatos = new List<Contato>
                    {
                        new Contato
                        {
                            Tipo = "WhatsApp",
                            Texto = "(31) 98888-7777"
                        },
                        new Contato
                        {
                            Tipo = "Email",
                            Texto = "contato@techcorp.com.br"
                        }
                    }
                }
            };

            context.Clientes.AddRange(clientesExemplo);
            context.SaveChanges();
        }
    }
}
