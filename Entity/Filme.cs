using Locadora_API.Enums;
using System;
using System.Collections.Generic;

namespace Locadora_API.Entity
{
    public class Filme
    {
        protected Filme() { }
        public Filme(string nome, string genero, int ano)
        {
            Nome = nome;
            Genero = genero;
            Ano = ano;
            Status = StatusFilmeEnum.Disponível;
            Locacoes = new List<Locacao>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Genero { get; set; }
        public int Ano { get; set; }
        public StatusFilmeEnum Status { get; set; }
        public List<Locacao> Locacoes { get; set; }
        public EstoqueFilme EstoqueFilme { get; set; }

        public string MudarStatusFilme(int statusId)
        {
            if (statusId == 0)
            {
                Status = StatusFilmeEnum.Disponível;
            }
            else if (statusId == 1)
            {
                Status = StatusFilmeEnum.Alugado;
            }
            else if (statusId == 2)
            {
                Status = StatusFilmeEnum.Removido;
            }
            else
            {
                throw new Exception("Status Inválido.");
            }

            return "Status alterado com sucesso!!!";
        }
    }
}
