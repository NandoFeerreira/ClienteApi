using ClienteApi.API.Responses;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Queries.Cliente;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClienteApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/clientes")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lista todos os clientes cadastrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        /// <response code="200">Lista retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet(Name = "ListarClientes")]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> ListarTodos()
        {
            try
            {
                var query = new GetAllClientesQuery();
                var clientes = await _mediator.Send(query);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar clientes");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }

        /// <summary>
        /// Busca um cliente específico por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Cliente encontrado</returns>
        /// <response code="200">Cliente encontrado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}", Name = "ObterClientePorId")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteDto>> ObterPorId(string id)
        {
            try
            {
                var query = new GetClienteByIdQuery { Id = id };
                var cliente = await _mediator.Send(query);

                if (cliente == null)
                    return NotFound(new ErrorResponse($"Cliente com ID {id} não encontrado", StatusCodes.Status404NotFound));

                return Ok(cliente);
            }
            catch (FormatException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }

        /// <summary>
        /// Pesquisa clientes por nome (busca parcial, case-insensitive)
        /// </summary>
        /// <param name="nome">Nome ou parte do nome</param>
        /// <returns>Lista de clientes encontrados</returns>
        /// <response code="200">Pesquisa realizada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("pesquisar", Name = "PesquisarClientes")]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Pesquisar([FromQuery] string nome)
        {
            try
            {
                var query = new SearchClientesQuery { Nome = nome };
                var clientes = await _mediator.Send(query);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar clientes: {Nome}", nome);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="command">Dados do cliente</param>
        /// <returns>Cliente criado</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost(Name = "CriarCliente")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteDto>> Criar([FromBody] CreateClienteCommand command)
        {
            try
            {
                var cliente = await _mediator.Send(command);
                return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validação falhou ao criar cliente");
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="command">Dados do cliente a ser atualizado</param>
        /// <returns>Cliente atualizado</returns>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Cliente não encontrado</response>
        [HttpPut("{id}", Name = "AtualizarCliente")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteDto>> Atualizar(string id, [FromBody] UpdateClienteCommand command)
        {
            try
            {
                if (id != command.Id)
                    return BadRequest(new ErrorResponse("ID da URL diferente do ID do corpo da requisição", StatusCodes.Status400BadRequest));

                var cliente = await _mediator.Send(command);
                return Ok(cliente);
            }
            catch (FormatException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(new ErrorResponse(ex.Message, StatusCodes.Status404NotFound));

                _logger.LogWarning(ex, "Validação falhou ao atualizar cliente {Id}", id);
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }

        /// <summary>
        /// Exclui um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Cliente excluído com sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}", Name = "ExcluirCliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Excluir(string id)
        {
            try
            {
                var command = new DeleteClienteCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new ErrorResponse($"Cliente com ID {id} não encontrado", StatusCodes.Status404NotFound));

                return NoContent();
            }
            catch (FormatException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse("Erro ao processar sua solicitação", StatusCodes.Status500InternalServerError));
            }
        }
    }
}
