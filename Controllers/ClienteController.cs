using Locadora_API.Entity;
using Locadora_API.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Locadora_API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly LocadoraDbContext _locadoraDbContext;

        public ClienteController(LocadoraDbContext locadoraDbContext)
        {
            _locadoraDbContext = locadoraDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var clientes = _locadoraDbContext
                .Clientes
                .Where(c => c.Status == 0) // BUSCAR SÓ USUÁRIOS ATIVOS
                .ToList();

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var clientes = _locadoraDbContext.Clientes.SingleOrDefault(c => c.Id == id);

            if (clientes == null)
            {
                return NotFound();
            }
            return Ok(clientes);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Cliente cliente)
        {
            _locadoraDbContext.Clientes.Add(cliente);
            _locadoraDbContext.SaveChanges();
            return NoContent();
        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] Cliente cliente)
        {
            if (!_locadoraDbContext.Clientes.Any(c => c.Id == id))
            {
                return NotFound(); // CLIENTE NÃO ENCONTRADO
            }

            _locadoraDbContext.Clientes.Update(cliente);
            _locadoraDbContext.SaveChanges();
           
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cliente = _locadoraDbContext.Clientes.SingleOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            cliente.MudarStatusCliente(1);
            _locadoraDbContext.SaveChanges();

            return NoContent();
        }

        }
}
