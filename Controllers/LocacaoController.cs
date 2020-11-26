using Locadora_API.Entity;
using Locadora_API.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locadora_API.Controllers
{
    [ApiController]
    [Route("api/locacoes")]
    public class LocacaoController : ControllerBase
    {
        private readonly LocadoraDbContext _locadoraDbContext;
        public LocacaoController(LocadoraDbContext locadoraDbContext)
        {
            _locadoraDbContext = locadoraDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var locacoes = (from locacao in _locadoraDbContext.Locacao
                                join filme in _locadoraDbContext.Filmes on locacao.FilmeId equals filme.Id
                                join cliente in _locadoraDbContext.Clientes on locacao.ClienteId equals cliente.Id                                
                                select new
                                {
                                    FilmeAlugado = filme.Nome,
                                    Genero = filme.Genero,
                                    Ano = filme.Ano,
                                    NomeCliente = cliente.Nome,
                                    Email = cliente.Email,
                                    DataAlocacao = locacao.DataAlocacao,
                                    DataDevolucaoPrevista = locacao.DataDevolucaoPrevista,
                                    Status = locacao.Status
                                }).ToList();

                return Ok(locacoes);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpGet]
        [Route("listaClientesAtivos")]
        public IActionResult GetByClientesAtivos()
        {
            try
            {
                var clientes = (from cliente in _locadoraDbContext.Clientes
                                where cliente.Status == 0
                                select new
                                {
                                    Id = cliente.Id,
                                    NomeCliente = cliente.Nome,
                                    Email = cliente.Email
                                }).ToList();

                return Ok(clientes);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpGet]
        [Route("listaFilmesDisponiveis")]
        public IActionResult GetByFilmesDisponiveis()
        {
            try
            {
                var filmes_estoque = (from filme in _locadoraDbContext.Filmes
                                      join estoque_filme in _locadoraDbContext.EstoqueFilme on filme.Id equals estoque_filme.FilmeId
                                      where filme.Status == 0
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

        [HttpPost]
        [Route("alugar")]
        public IActionResult Post(int clienteId, int filmeId, DateTime dataAlocacao, DateTime dataDevolucaoPrevista)
        {
            try
            {
                // verificar se existe cliente
                var cliente = _locadoraDbContext.Clientes.SingleOrDefault(c => c.Id == clienteId);
                // verificar se existe filme
                var filme = _locadoraDbContext.Filmes.SingleOrDefault(c => c.Id == filmeId);

                if (cliente == null || filme == null)
                {
                    throw new Exception("Cliente ou Filme não encontrado. Por favor, verifique e insere novamente");
                }

                // BUSCAR A QUANTIDADE DE ESTOQUE DO FILME ESPECÍFICO
                var estoqueFilme = _locadoraDbContext.EstoqueFilme.SingleOrDefault(e => e.FilmeId == filme.Id);

                if (estoqueFilme == null)
                {
                    throw new Exception("Ops! Filme registrado no sistema sem quantidade de estoque definido. Por favor, atualizar a quantidade para prosseguir!!!");
                }                

                if (estoqueFilme.Quantidade <= 0)
                {
                    throw new Exception("Ops! A quantidade solicitada de filme excede o estoque dos nossos bancos de dados. Verifique a quantidade novamente e tente mais tarde. Obrigado!!!");
                }
                else
                {
                    // DECREMENTAR QUANTIDADE FILME
                    estoqueFilme.DecrementarQuantidadeEstoque();

                    if (estoqueFilme.Quantidade == 0)
                    {
                        filme.MudarStatusFilme(1); // MUDAR STATUS PARA 1 DE ALUGADO
                        _locadoraDbContext.Filmes.Update(filme); //SALVAR DADOS NO BANCO DE DADOS
                        _locadoraDbContext.SaveChanges();
                    }                    
                    _locadoraDbContext.EstoqueFilme.Update(estoqueFilme);
                    _locadoraDbContext.SaveChanges();


                    Locacao locacao = new Locacao
                    {
                        ClienteId = cliente.Id,
                        FilmeId = filme.Id,
                        DataAlocacao = dataAlocacao,
                        DataDevolucaoPrevista = dataDevolucaoPrevista
                    };

                    _locadoraDbContext.Locacao.Add(locacao);
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
        [Route("devolver")]
        public IActionResult Put(int locacaoId, int clienteId, int filmeId, DateTime DataDevolucaoEntregaPeloCliente)
        {
            try
            {
                string mensagemAviso = null;
                // verificar Locacao && l.Status == 0
                var locacao = _locadoraDbContext.Locacao
                    .Where(l => l.ClienteId == clienteId && l.FilmeId == filmeId) // SÓ BUSCAR FILMES QUE ESTÃO ALUGADOS
                    .SingleOrDefault(l => l.Id == locacaoId);

                if (locacao == null)
                {
                    throw new Exception("Dados de Locação Incorretos. Por favor, verifique e insere novamente os dados corretos");
                }

                // BUSCAR A QUANTIDADE DE ESTOQUE DO FILME ESPECÍFICO
                var estoqueFilme = _locadoraDbContext.EstoqueFilme.SingleOrDefault(e => e.FilmeId == locacao.FilmeId);

                estoqueFilme.AumentarQuantidadeEstoque();
                if (estoqueFilme.Quantidade > 0)
                {
                    var filme = _locadoraDbContext.Filmes.SingleOrDefault(c => c.Id == locacao.FilmeId);

                    filme.MudarStatusFilme(0);
                    _locadoraDbContext.Filmes.Update(filme);
                    _locadoraDbContext.SaveChanges();
                }

                _locadoraDbContext.EstoqueFilme.Update(estoqueFilme);
                _locadoraDbContext.SaveChanges();

                if (DataDevolucaoEntregaPeloCliente > locacao.DataDevolucaoPrevista)
                {
                    // DEVOLUÇÃO COM ATRASO
                    locacao.MudarStatusLocacao(2); // STATUS DE DEVOLVIDO COM ATRASO
                    mensagemAviso = "Aviso. Devolução de filme com atraso.";
                } else
                {
                    locacao.MudarStatusLocacao(1); // STATUS DEFINIDO EM StatusLocacaoEnum
                }
                
                locacao.DataDevolucaoEntregaPeloCliente = DataDevolucaoEntregaPeloCliente;

                _locadoraDbContext.Locacao.Update(locacao);
                _locadoraDbContext.SaveChanges();

                if (mensagemAviso != null)
                {
                    return Ok(mensagemAviso);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var locacao = _locadoraDbContext.Locacao.SingleOrDefault(l => l.Id == id);

                if (locacao == null)
                {
                    return NotFound();
                }

                locacao.MudarStatusLocacao(3); // STATUS DE EXCLUIDO NO ENUM
                _locadoraDbContext.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro: {e}");
            }
        }
    }
}
