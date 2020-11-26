using Locadora_API.Enums;
using System;

namespace Locadora_API.Entity
{
    public class Locacao
    {
        public Locacao() { }
        public Locacao(int clienteId, int filmeId, DateTime dataAlocacao, DateTime dataDevolucaoPrevista)
        {
            ClienteId = clienteId;
            FilmeId = filmeId;
            DataAlocacao = dataAlocacao;
            DataDevolucaoPrevista = dataDevolucaoPrevista;
            Status = StatusLocacaoEnum.Alugado;
        }
        public int Id { get; private set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int FilmeId { get; set; }
        public Filme Filme { get; set; }
        public DateTime DataAlocacao { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
        public DateTime DataDevolucaoEntregaPeloCliente { get;  set; }
        public StatusLocacaoEnum Status { get; set; }

        public string MudarStatusLocacao(int statusId)
        {
            if (statusId == 0)
            {
                Status = StatusLocacaoEnum.Alugado;
            }
            else if (statusId == 1)
            {
                Status = StatusLocacaoEnum.Devolvido;
            }
            else if (statusId == 2)
            {
                Status = StatusLocacaoEnum.DevolvidoComAtrasado;
            }
            else if (statusId == 3)
            {
                Status = StatusLocacaoEnum.Excluido;
            }
            else
            {
                throw new Exception("Status Inválido.");
            }

            return "Status alterado com sucesso!!!";
        }

    }
}
