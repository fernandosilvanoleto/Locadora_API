using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Locadora_API.Entity;
using Locadora_API.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Locadora_API.Controllers
{
    [ApiController]
    [Route("api/filmes")]
    public class FilmeController : ControllerBase
    {
        private readonly LocadoraDbContext _locadoraDbContext;
        public FilmeController(LocadoraDbContext locadoraDbContext)
        {
            _locadoraDbContext = locadoraDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {

                var filmes_estoque = (from filme in _locadoraDbContext.Filmes
                                      join estoque_filme in _locadoraDbContext.EstoqueFilme on filme.Id equals estoque_filme.FilmeId
                                      select new
                                      {
                                          FilmeId = filme.Id,
                                          Nome = filme.Nome,
                                          Genero = filme.Genero,
                                          Ano = filme.Ano,
                                          Status = filme.Status,
                                          QuantidadeNoEstoque = estoque_filme.Quantidade
                                      }).ToList();

                return Ok(filmes_estoque);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }

        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var filmes = _locadoraDbContext.Filmes.SingleOrDefault(f => f.Id == id);

                if (filmes == null)
                {
                    return NotFound();
                }

                return Ok(filmes);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpPost]
        public IActionResult Post(int quantidadeEstoqueFilmeInicial, [FromBody] Filme filme)
        {
            try
            {
                if (quantidadeEstoqueFilmeInicial <= 0)
                {
                    throw new Exception("Por favor, inserir pelo menos um filme no estoque da locadora. Verifique a quantidade corretamente!!!.");
                }
                else
                {
                    // SALVAR FILME
                    _locadoraDbContext.Filmes.Add(filme);
                    _locadoraDbContext.SaveChanges();

                    int filmeId = filme.Id;

                    EstoqueFilme estoqueFilme = new EstoqueFilme(filmeId, quantidadeEstoqueFilmeInicial);

                    // SALVAR ESTOQUE DO FILME COM O ID DO FILME QUE ACABOU DE SER INSERIDO
                    _locadoraDbContext.EstoqueFilme.Add(estoqueFilme);
                    _locadoraDbContext.SaveChanges();
                }


                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpPut]
        public IActionResult Put(int filmeId, int quantidadeEstoqueFilmeInicial, [FromBody] Filme filme)
        {
            try
            {
                if (quantidadeEstoqueFilmeInicial <= 0)
                {
                    throw new Exception("Por favor, inserir pelo menos um filme no estoque da locadora. Verifique a quantidade corretamente!!!.");
                }
                else
                {
                    if (!_locadoraDbContext.Filmes.Any(c => c.Id == filmeId))
                    {
                        return NotFound(); // FILME NÃO ENCONTRADO
                    }

                    _locadoraDbContext.Filmes.Update(filme);
                    _locadoraDbContext.SaveChanges();

                    return NoContent();
                }
                
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var filme = _locadoraDbContext.Filmes.SingleOrDefault(c => c.Id == id);

            if (filme == null)
            {
                return NotFound();
            }

            filme.MudarStatusFilme(2);
            _locadoraDbContext.SaveChanges();

            return NoContent();
        }

    }
}

