# ClienteApi - API de Gerenciamento de Clientes

API RESTful desenvolvida em .NET 8.0 para gerenciamento completo de clientes, endereços e contatos, com integração automática à API ViaCEP para preenchimento de dados de endereço.

## Sumário

- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Configuração do Ambiente](#configuração-do-ambiente)
  - [Pré-requisitos](#pré-requisitos)
  - [Opção 1: Banco de Dados InMemory (Padrão)](#opção-1-banco-de-dados-inmemory-padrão)
  - [Opção 2: PostgreSQL com Docker](#opção-2-postgresql-com-docker)
  - [Opção 3: PostgreSQL com Podman](#opção-3-postgresql-com-podman)
  - [Opção 4: PostgreSQL Próprio (Instância Local)](#opção-4-postgresql-próprio-instância-local)
- [Executando a Aplicação](#executando-a-aplicação)
- [Documentação da API](#documentação-da-api)
- [Endpoints Disponíveis](#endpoints-disponíveis)
- [Exemplos de Uso](#exemplos-de-uso)
- [Executando os Testes](#executando-os-testes)
- [Modelo de Dados](#modelo-de-dados)

---

## Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - API Web
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Banco de dados (opcional)
- **AutoMapper** - Mapeamento de objetos
- **MediatR** - Padrão CQRS
- **FluentValidation** - Validações
- **Swagger/OpenAPI** - Documentação
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions para testes

## Arquitetura do Projeto

O projeto segue os princípios de Clean Architecture e está organizado em camadas:

```
ClienteApi/
├── ClienteApi.Domain/          # Entidades de negócio e interfaces
├── ClienteApi.Application/     # Casos de uso, DTOs, validações
├── ClienteApi.Infrastructure/  # Implementações de acesso a dados
├── ClienteApi.API/            # Controllers e configurações da API
├── ClienteApi.Tests.Unit/     # Testes unitários
├── ClienteApi.Tests.Integration/ # Testes de integração
└── ClienteApi.Infrastructure.Tests/ # Testes de infraestrutura
```

**Padrões implementados:**
- Repository Pattern
- Unit of Work
- CQRS (Command Query Responsibility Segregation)
- Dependency Injection
- DTO (Data Transfer Object)

---

## Configuração do Ambiente

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- Editor de código (Visual Studio 2022, VS Code, Rider, etc.)
- Git (para clonar o repositório)

### Opção 1: Banco de Dados InMemory (Padrão)

A aplicação está configurada por padrão para usar banco de dados em memória, ideal para testes rápidos e avaliação sem necessidade de configuração adicional.

**Nenhuma configuração necessária!** Apenas execute a aplicação.

A aplicação iniciará com dados de exemplo (seed) automaticamente.

---

### Opção 2: PostgreSQL com Docker

#### Passo 1: Instalar o Docker

**Windows:**
1. Baixe o [Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. Execute o instalador e siga as instruções
3. Reinicie o computador se solicitado
4. Abra o Docker Desktop e aguarde inicializar

**macOS:**
1. Baixe o [Docker Desktop para Mac](https://www.docker.com/products/docker-desktop/)
2. Abra o arquivo .dmg e arraste para Applications
3. Execute o Docker Desktop

#### Passo 2: Subir o PostgreSQL com Docker

Execute o comando abaixo para criar e iniciar o container PostgreSQL com as configurações do projeto:

```bash
docker run -d \
  --name desafiodb-postgres \
  -e POSTGRES_DB=desafiodb \
  -e POSTGRES_USER=desafio \
  -e POSTGRES_PASSWORD=desafio123 \
  -p 5432:5432 \
  postgres:15
```

**Explicação dos parâmetros:**
- `-d`: Executa em background (detached)
- `--name`: Nome do container
- `-e POSTGRES_DB`: Nome do banco de dados
- `-e POSTGRES_USER`: Usuário do banco
- `-e POSTGRES_PASSWORD`: Senha do usuário
- `-p 5432:5432`: Mapeia a porta do container para o host
- `postgres:15`: Imagem e versão do PostgreSQL

#### Passo 3: Verificar se o container está rodando

```bash
docker ps
```

Você deve ver o container `desafiodb-postgres` na lista.

#### Comandos úteis do Docker:

```bash
# Parar o container
docker stop desafiodb-postgres

# Iniciar o container
docker start desafiodb-postgres

# Ver logs do container
docker logs desafiodb-postgres

# Remover o container (quando não precisar mais)
docker rm -f desafiodb-postgres
```

---

### Opção 3: PostgreSQL com Podman

Podman é uma alternativa ao Docker

#### Passo 1: Instalar o Podman

**Windows:**
1. Baixe o [Podman Desktop](https://podman-desktop.io/)
2. Execute o instalador
3. Siga as instruções de configuração

**macOS:**
```bash
brew install podman
podman machine init
podman machine start
```

#### Passo 2: Subir o PostgreSQL com Podman

```bash
podman run -d \
  --name desafiodb-postgres \
  -e POSTGRES_DB=desafiodb \
  -e POSTGRES_USER=desafio \
  -e POSTGRES_PASSWORD=desafio123 \
  -p 5432:5432 \
  postgres:15
```

#### Passo 3: Verificar se o container está rodando

```bash
podman ps
```

#### Comandos úteis do Podman:

```bash
# Parar o container
podman stop desafiodb-postgres

# Iniciar o container
podman start desafiodb-postgres

# Ver logs
podman logs desafiodb-postgres

# Remover o container
podman rm -f desafiodb-postgres
```

---

### Opção 4: PostgreSQL Próprio (Instância Local)

Se você já possui uma instância do PostgreSQL instalada localmente ou em um servidor, siga estes passos:

#### Passo 1: Criar o banco de dados e usuário

Conecte-se ao PostgreSQL como administrador e execute:

```sql
-- Criar usuário
CREATE USER desafio WITH PASSWORD 'desafio123';

-- Criar banco de dados
CREATE DATABASE desafiodb OWNER desafio;

-- Conceder privilégios
GRANT ALL PRIVILEGES ON DATABASE desafiodb TO desafio;
```

#### Passo 2: Verificar configurações de conexão

Certifique-se de que o PostgreSQL está:
- Aceitando conexões na porta 5432 (ou ajuste no appsettings.json)
- Permitindo conexões locais (verifique `pg_hba.conf`)
- O serviço está rodando

---

### Configurar a Aplicação para Usar PostgreSQL

Após configurar o PostgreSQL (Docker, Podman ou instância própria), edite o arquivo `appsettings.json`:

**Localização:** `ClienteApi.API/appsettings.json`

```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=desafiodb;Username=desafio;Password=desafio123"
  }
}
```

**Parâmetros da string de conexão:**
- `Host`: Endereço do servidor (localhost para local)
- `Port`: Porta do PostgreSQL (padrão: 5432)
- `Database`: Nome do banco de dados
- `Username`: Usuário do banco
- `Password`: Senha do usuário

**Para servidor remoto, ajuste o Host:**
```json
"DefaultConnection": "Host=192.168.1.100;Port=5432;Database=desafiodb;Username=desafio;Password=desafio123"
```

---

## Executando a Aplicação

### Passo 1: Clonar o repositório (se ainda não clonou)

```bash
git clone <url-do-repositorio>
cd ClienteApi
```

### Passo 2: Restaurar dependências

```bash
dotnet restore
```

### Passo 3: Atualizar o Banco de Dados (somente se usar PostgreSQL)

As migrações do banco de dados já estão prontas no projeto. Você só precisa executar o comando abaixo para criar o esquema (tabelas, etc.) no seu banco de dados PostgreSQL.

```bash
# Navegue até a pasta de infraestrutura
cd ClienteApi.Infrastructure

# Aplique a migração
dotnet ef database update --startup-project ../ClienteApi.API

# Volte para a pasta raiz
cd ..
```

**Nota:** Se você não tiver o EF Core Tools instalado globalmente, instale-o primeiro com o comando:
```bash
dotnet tool install --global dotnet-ef
```

### Passo 4: Executar a aplicação

```bash
cd ClienteApi.API
dotnet run
```

A aplicação estará disponível em duas URLs:
- **HTTPS:** https://localhost:5501
- **HTTP:** http://localhost:5500

Ao iniciar, o navegador abrirá automaticamente a documentação do Swagger.

---

## Documentação da API

A documentação interativa da API (Swagger UI) é aberta automaticamente ao iniciar a aplicação.

**URL do Swagger:** https://localhost:5501/swagger

As requisições da API podem ser feitas tanto para a URL HTTPS (`https://localhost:5501`) quanto para a HTTP (`http://localhost:5500`).

O Swagger permite:
- Visualizar todos os endpoints
- Testar requisições diretamente no navegador
- Ver modelos de dados (schemas)
- Consultar códigos de resposta HTTP

---

## Endpoints Disponíveis

### Base URL
```
http://localhost:5500/api/v1/clientes
```

### Lista de Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/clientes` | Lista todos os clientes |
| GET | `/api/v1/clientes/{id}` | Busca cliente por ID |
| GET | `/api/v1/clientes/pesquisar?nome={nome}` | Pesquisa clientes por nome |
| POST | `/api/v1/clientes` | Cria um novo cliente |
| PUT | `/api/v1/clientes/{id}` | Atualiza um cliente existente |
| DELETE | `/api/v1/clientes/{id}` | Exclui um cliente |

---

## Exemplos de Uso

### 1. Listar Todos os Clientes

**Requisição:**
```bash
curl -X GET "http://localhost:5500/api/v1/clientes"
```

**Resposta (200 OK):**
```json
[
  {
    "id": "AgAAAA",
    "nome": "Maria Santos",
    "dataCadastro": "2025-10-14T11:23:40.885829-03:00",
    "enderecos": [
      {
        "id": "AgAAAA",
        "cep": "20040020",
        "logradouro": "Avenida Rio Branco",
        "cidade": "Rio de Janeiro",
        "numero": "200",
        "complemento": "Sala 501"
      }
    ],
    "contatos": [
      {
        "id": "AwAAAA",
        "tipo": "Celular",
        "texto": "21999998888"
      },
      {
        "id": "BAAAAA",
        "tipo": "Email",
        "texto": "maria.santos@empresa.com"
      }
    ]
  },
  {
    "id": "AQAAAA",
    "nome": "João Silva",
    "dataCadastro": "2025-09-29T11:23:40.8843997-03:00",
    "enderecos": [
      {
        "id": "AQAAAA",
        "cep": "01001000",
        "logradouro": "Praça da Sé",
        "cidade": "São Paulo",
        "numero": "100",
        "complemento": "Apto 101"
      }
    ],
    "contatos": [
      {
        "id": "AQAAAA",
        "tipo": "Telefone",
        "texto": "11987654321"
      },
      {
        "id": "AgAAAA",
        "tipo": "Email",
        "texto": "joao.silva@email.com"
      }
    ]
  }
]
```

---

### 2. Buscar Cliente por ID

**Requisição:**
```bash
curl -X GET "http://localhost:5500/api/v1/clientes/AQAAAA"
```

**Resposta (200 OK):**
```json
{
  "id": "AQAAAA",
  "nome": "João Silva",
  "dataCadastro": "2025-09-29T11:23:40.8843997-03:00",
  "enderecos": [
    {
      "id": "AQAAAA",
      "cep": "01001000",
      "logradouro": "Praça da Sé",
      "cidade": "São Paulo",
      "numero": "100",
      "complemento": "Apto 101"
    }
  ],
  "contatos": [
    {
      "id": "AQAAAA",
      "tipo": "Telefone",
      "texto": "11987654321"
    },
    {
      "id": "AgAAAA",
      "tipo": "Email",
      "texto": "joao.silva@email.com"
    }
  ]
}
```

**Resposta (404 Not Found):**
```json
{
  "message": "Cliente com ID ZZZZZZ não encontrado",
  "statusCode": 404
}
```

---

### 3. Pesquisar Clientes por Nome

**Requisição:**
```bash
curl -X GET "http://localhost:5500/api/v1/clientes/pesquisar?nome=João"
```

**Resposta (200 OK):**
```json
[
  {
    "id": "AQAAAA",
    "nome": "João Silva",
    "dataCadastro": "2025-09-29T11:23:40.8843997-03:00",
    "enderecos": [
      {
        "id": "AQAAAA",
        "cep": "01001000",
        "logradouro": "Praça da Sé",
        "cidade": "São Paulo",
        "numero": "100",
        "complemento": "Apto 101"
      }
    ],
    "contatos": [
      {
        "id": "AQAAAA",
        "tipo": "Telefone",
        "texto": "11987654321"
      },
      {
        "id": "AgAAAA",
        "tipo": "Email",
        "texto": "joao.silva@email.com"
      }
    ]
  }
]
```

---

### 4. Criar Novo Cliente

#### Exemplo 1: Cliente Básico (Mínimo Necessário)

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Maria Santos",
    "enderecos": [
      {
        "cep": "01310-100",
        "numero": "1500"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "maria@exemplo.com"
      }
    ]
  }'
```

**Resposta (201 Created):**
```json
{
  "id": "BQAAAA",
  "nome": "Maria Santos",
  "dataCadastro": "2025-10-29T14:24:44.5888756Z",
  "enderecos": [
    {
      "id": "BgAAAA",
      "cep": "01310-100",
      "logradouro": "Avenida Paulista",
      "cidade": "São Paulo",
      "numero": "1500",
      "complemento": "de 612 a 1510 - lado par"
    }
  ],
  "contatos": [
    {
      "id": "CQAAAA",
      "tipo": "Email",
      "texto": "maria@exemplo.com"
    }
  ]
}
```

#### Exemplo 2: Cliente Completo (Múltiplos Endereços e Contatos)

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Carlos Oliveira",
    "enderecos": [
      {
        "cep": "20040-020",
        "numero": "156",
        "complemento": "Sala 203"
      },
      {
        "cep": "22640-102",
        "numero": "89",
        "complemento": "Casa"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "carlos@empresa.com.br"
      },
      {
        "tipo": "Celular",
        "texto": "21987654321"
      },
      {
        "tipo": "Telefone",
        "texto": "2133334444"
      },
      {
        "tipo": "WhatsApp",
        "texto": "21987654321"
      }
    ]
  }'
```

**Resposta (201 Created):**
```json
{
  "id": "BQAAAA",
  "nome": "Carlos Oliveira",
  "dataCadastro": "2025-10-29T14:35:00Z",
  "enderecos": [
    {
      "id": "BgAAAA",
      "cep": "20040020",
      "logradouro": "Avenida Rio Branco",
      "cidade": "Rio de Janeiro",
      "numero": "156",
      "complemento": "Sala 203"
    },
    {
      "id": "BwAAAA",
      "cep": "22640102",
      "logradouro": "Avenida das Américas",
      "cidade": "Rio de Janeiro",
      "numero": "89",
      "complemento": "Casa"
    }
  ],
  "contatos": [
    {
      "id": "CQAAAA",
      "tipo": "Email",
      "texto": "carlos@empresa.com.br"
    },
    {
      "id": "CgAAAA",
      "tipo": "Celular",
      "texto": "21987654321"
    },
    {
      "id": "CwAAAA",
      "tipo": "Telefone",
      "texto": "2133334444"
    },
    {
      "id": "DAAAAA",
      "tipo": "WhatsApp",
      "texto": "21987654321"
    }
  ]
}
```

#### Exemplo 3: CEP Inválido (Teste de Erro)

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Erro",
    "enderecos": [
      {
        "cep": "99999-999",
        "numero": "123"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "teste@exemplo.com"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "message": "CEP '99999-999' não encontrado",
  "statusCode": 400
}
```

#### Exemplo 4: Validação - Nome Vazio

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "",
    "enderecos": [
      {
        "cep": "01001-000",
        "numero": "100"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "teste@exemplo.com"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": [
      "O nome do cliente é obrigatório",
      "O nome deve ter no mínimo 3 caracteres"
    ]
  },
  "traceId": "00-aa666ebc9aa58ee3534a6531bcb2f326-2b59d4dd3a1ed819-00"
}
```

#### Exemplo 5: Validação - Endereços Duplicados

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Duplicado",
    "enderecos": [
      {
        "cep": "01001-000",
        "numero": "100"
      },
      {
        "cep": "01001-000",
        "numero": "100"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "teste@exemplo.com"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Enderecos": [
      "Não é permitido cadastrar o mesmo CEP e número mais de uma vez."
    ]
  },
  "traceId": "00-f53794e6886e7bfad785ec8f80540178-883500cc927a852a-00"
}
```

#### Exemplo 6: Validação - Contatos Duplicados

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Contato Duplicado",
    "enderecos": [
      {
        "cep": "01001-000",
        "numero": "100"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "teste@exemplo.com"
      },
      {
        "tipo": "Celular",
        "texto": "teste@exemplo.com"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Contatos[1].Texto": [
      "Telefone/Celular deve estar em um formato válido, como (00) 90000-0000."
    ]
  },
  "traceId": "00-0552f8205cf5311bed513ecb231ba3c5-7765d8e8ef0b1809-00"
}
```

**Nota:** Este exemplo específico retorna erro de formato porque "teste@exemplo.com" não é um formato válido de celular. Para testar duplicação, use o payload correto fornecido em `test-payload-erro-contato-duplicado.json`

#### Exemplo 7: Validação - Email Inválido

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Email",
    "enderecos": [
      {
        "cep": "01001-000",
        "numero": "100"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "email-invalido"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Contatos[0].Texto": [
      "Email inválido"
    ]
  },
  "traceId": "00-eae08c101910935ed9ca6c872fac7a2d-3f463c71790f1997-00"
}
```

#### Exemplo 8: Validação - Telefone Inválido

**Requisição:**
```bash
curl -X POST "http://localhost:5500/api/v1/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Telefone",
    "enderecos": [
      {
        "cep": "01001-000",
        "numero": "100"
      }
    ],
    "contatos": [
      {
        "tipo": "Celular",
        "texto": "123"
      }
    ]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Contatos[0].Texto": [
      "Telefone/Celular deve estar em um formato válido, como (00) 90000-0000."
    ]
  },
  "traceId": "00-6d3a28271e8d9633596cc798f593b52e-a0336022ff9c5fd0-00"
}
```

---

### 5. Atualizar Cliente

#### Exemplo 1: Atualização Básica

**Requisição:**
```bash
curl -X PUT "http://localhost:5500/api/v1/clientes/AgAAAA" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "AgAAAA",
    "nome": "Maria Santos Atualizada",
    "enderecos": [
      {
        "cep": "01001000",
        "numero": "200"
      }
    ],
    "contatos": [
      {
        "tipo": "Email",
        "texto": "maria.nova@exemplo.com"
      }
    ]
  }'
```

**Resposta (200 OK):**
```json
{
  "id": "AgAAAA",
  "nome": "Maria Santos Atualizada",
  "dataCadastro": "2025-10-14T12:07:38.059095-03:00",
  "enderecos": [
    {
      "id": "BAAAAA",
      "cep": "01001000",
      "logradouro": "Praça da Sé",
      "cidade": "São Paulo",
      "numero": "200"
    }
  ],
  "contatos": [
    {
      "id": "BwAAAA",
      "tipo": "Email",
      "texto": "maria.nova@exemplo.com"
    }
  ]
}
```

#### Exemplo 2: Atualizar Mantendo Alguns Itens

**Requisição:**
```bash
curl -X PUT "http://localhost:5500/api/v1/clientes/AQAAAA" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "AQAAAA",
    "nome": "João Silva Atualizado",
    "enderecos": [
      {
        "id": "AQAAAA",
        "cep": "01001-000",
        "numero": "200"
      },
      {
        "cep": "01310-100",
        "numero": "1500"
      }
    ],
    "contatos": [
      {
        "id": "AQAAAA",
        "tipo": "Telefone",
        "texto": "11987654321"
      },
      {
        "tipo": "Celular",
        "texto": "11999998888"
      }
    ]
  }'
```

**Nota:** Quando você inclui o `id` em endereços/contatos, o sistema atualiza o item existente. Sem o `id`, cria um novo item.

**Resposta (200 OK):**
```json
{
  "id": "AQAAAA",
  "nome": "João Silva Atualizado",
  "dataCadastro": "2025-09-29T11:23:40.8843997-03:00",
  "enderecos": [
    {
      "id": "AQAAAA",
      "cep": "01001-000",
      "logradouro": "Praça da Sé",
      "cidade": "São Paulo",
      "numero": "200",
      "complemento": null
    },
    {
      "id": "CwAAAA",
      "cep": "01310-100",
      "logradouro": "Avenida Paulista",
      "cidade": "São Paulo",
      "numero": "1500",
      "complemento": "de 612 a 1510 - lado par"
    }
  ],
  "contatos": [
    {
      "id": "AQAAAA",
      "tipo": "Telefone",
      "texto": "11987654321"
    },
    {
      "id": "CwAAAA",
      "tipo": "Celular",
      "texto": "11999998888"
    }
  ]
}
```

#### Exemplo 3: ID da URL Diferente do Body (Erro)

**Requisição:**
```bash
curl -X PUT "http://localhost:5500/api/v1/clientes/AQAAAA" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "AgAAAA",
    "nome": "Teste",
    "enderecos": [{"cep": "01001-000", "numero": "100"}],
    "contatos": [{"tipo": "Email", "texto": "teste@exemplo.com"}]
  }'
```

**Resposta (400 Bad Request):**
```json
{
  "message": "ID da URL diferente do ID do corpo da requisição",
  "statusCode": 400
}
```

---

### 6. Excluir Cliente

**Requisição:**
```bash
curl -X DELETE "http://localhost:5500/api/v1/clientes/AwAAAA"
```

**Resposta (204 No Content):**
```
(corpo vazio - apenas HTTP 204)
```

**Resposta (404 Not Found):**
```json
{
  "message": "Cliente com ID ZZZZZZ não encontrado",
  "statusCode": 404
}
```

---

## Payloads de Teste Completos

### Arquivo: `test-payload-valid.json`
Cliente válido completo:
```json
{
  "nome": "Fernando",
  "enderecos": [
    {
      "cep": "11619419",
      "numero": "700",
      "complemento": "casa 2"
    },
    {
      "cep": "11619419",
      "numero": "710",
      "complemento": "casa 3"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "fernando.dev@gmail.com"
    },
    {
      "tipo": "Celular",
      "texto": "13999999999"
    }
  ]
}
```

### Arquivo: `test-payload-erro-cep-invalido.json`
Teste de CEP inválido:
```json
{
  "nome": "Teste CEP Inválido",
  "enderecos": [
    {
      "cep": "99999-999",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "teste@exemplo.com"
    }
  ]
}
```

### Arquivo: `test-payload-erro-nome-vazio.json`
Teste de validação - nome vazio:
```json
{
  "nome": "",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "teste@exemplo.com"
    }
  ]
}
```

### Arquivo: `test-payload-erro-email-invalido.json`
Teste de validação - email inválido:
```json
{
  "nome": "Teste Email",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "email-sem-arroba"
    }
  ]
}
```

### Arquivo: `test-payload-erro-telefone-invalido.json`
Teste de validação - telefone inválido:
```json
{
  "nome": "Teste Telefone",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Celular",
      "texto": "123"
    }
  ]
}
```

### Arquivo: `test-payload-erro-endereco-duplicado.json`
Teste de validação - endereço duplicado:
```json
{
  "nome": "Teste Endereço Duplicado",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    },
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "teste@exemplo.com"
    }
  ]
}
```

### Arquivo: `test-payload-erro-contato-duplicado.json`
Teste de validação - contato duplicado:
```json
{
  "nome": "Teste Contato Duplicado",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "teste@exemplo.com"
    },
    {
      "tipo": "Celular",
      "texto": "teste@exemplo.com"
    }
  ]
}
```

### Arquivo: `test-payload-erro-tipo-contato-invalido.json`
Teste de validação - tipo de contato inválido:
```json
{
  "nome": "Teste Tipo Inválido",
  "enderecos": [
    {
      "cep": "01001-000",
      "numero": "100"
    }
  ],
  "contatos": [
    {
      "tipo": "Fax",
      "texto": "1234-5678"
    }
  ]
}
```

### Arquivo: `test-payload-multiplos-enderecos-contatos.json`
Cliente com múltiplos endereços e contatos:
```json
{
  "nome": "Ana Paula Costa",
  "enderecos": [
    {
      "cep": "01310100",
      "numero": "1578",
      "complemento": "Andar 12"
    },
    {
      "cep": "04543907",
      "numero": "2000",
      "complemento": "Bloco B"
    },
    {
      "cep": "05426000",
      "numero": "350"
    }
  ],
  "contatos": [
    {
      "tipo": "Email",
      "texto": "ana.costa@empresa.com.br"
    },
    {
      "tipo": "Email",
      "texto": "ana.pessoal@gmail.com"
    },
    {
      "tipo": "Celular",
      "texto": "11987654321"
    },
    {
      "tipo": "Telefone",
      "texto": "1133334444"
    },
    {
      "tipo": "WhatsApp",
      "texto": "11987654321"
    }
  ]
}
```

---

## Executando os Testes

O projeto possui três tipos de testes: unitários, de integração e de infraestrutura.

### Executar todos os testes:

```bash
dotnet test
```

### Executar testes unitários:

```bash
dotnet test ClienteApi.Tests.Unit
```

### Executar testes de integração:

```bash
dotnet test ClienteApi.Tests.Integration
```

### Executar testes de infraestrutura:

```bash
dotnet test ClienteApi.Infrastructure.Tests
```

### Executar com relatório de cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Modelo de Dados

### Entidade Cliente

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único (auto-incremento) |
| Nome | string | Nome do cliente (obrigatório, 3-200 caracteres) |
| DataCadastro | DateTime | Data de cadastro (preenchido automaticamente) |
| Enderecos | List | Lista de endereços do cliente |
| Contatos | List | Lista de contatos do cliente |

### Entidade Endereco

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único (auto-incremento) |
| Cep | string | CEP do endereço (obrigatório, formato: 00000-000) |
| Logradouro | string | Nome da rua/avenida (preenchido via ViaCEP) |
| Cidade | string | Nome da cidade (preenchido via ViaCEP) |
| Numero | string | Número do imóvel (obrigatório) |
| Complemento | string | Complemento (opcional) |
| ClienteId | int | Chave estrangeira para Cliente |

### Entidade Contato

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único (auto-incremento) |
| Tipo | string | Tipo de contato (Email, Celular, Telefone, WhatsApp) |
| Texto | string | Valor do contato (obrigatório) |
| ClienteId | int | Chave estrangeira para Cliente |

### Diagrama de Relacionamentos

```
Cliente (1) ----< (N) Endereco
Cliente (1) ----< (N) Contato
```

---

## Regras de Validação

### Cliente
- Nome é obrigatório (mínimo 3, máximo 200 caracteres)
- Deve ter pelo menos um endereço
- Deve ter pelo menos um contato

### Endereço
- CEP é obrigatório e deve estar no formato 00000-000 ou 00000000
- CEP deve existir na base do ViaCEP
- Número é obrigatório
- Não é permitido cadastrar o mesmo CEP e número mais de uma vez para o mesmo cliente
- Complemento é opcional

### Contato
- Tipo é obrigatório (Email, Celular, Telefone, WhatsApp)
- Texto é obrigatório
- Email deve ser válido (quando tipo = Email)
- Telefone/Celular pode ser enviado com ou sem formatação (aceita: 11987654321 ou (11) 98765-4321)
- Não é permitido cadastrar o mesmo contato mais de uma vez (normalização inteligente)

---

## Observações Importantes

### Conversão de IDs
Os IDs são convertidos para Base64 URL-safe (sem padding) nas respostas da API por questões de segurança. Exemplo:
- ID no banco: `1` → ID na API: `AQAAAA`
- ID no banco: `2` → ID na API: `AgAAAA`
- ID no banco: `3` → ID na API: `AwAAAA`
- ID no banco: `4` → ID na API: `BAAAAA`

Os caracteres `=` são removidos (sem padding), e `+` e `/` são substituídos por `-` e `_` respectivamente (URL-safe).

Sempre use o ID retornado pela API nas requisições subsequentes.

### Integração ViaCEP
Quando um CEP é informado:
1. A API consulta automaticamente o ViaCEP
2. Preenche automaticamente `logradouro` e `cidade`
3. Se o CEP não existir, retorna erro 400
4. Você precisa fornecer apenas: `cep`, `numero`, e opcionalmente `complemento`

### Tipos de Contato Aceitos
Apenas os seguintes tipos são válidos:
- `Email`
- `Celular`
- `Telefone`
- `WhatsApp`

### Validação de Duplicação
- **Endereços:** Mesmo CEP + mesmo número = duplicado
- **Contatos:** Mesmo texto (após normalização) = duplicado
  - Emails são normalizados (lowercase, trim)
  - Telefones são normalizados (apenas dígitos)

---
