using Locadora_API.Enums;
using System;
using System.Collections.Generic;


namespace Locadora_API.Entity
{
    public class Cliente
    {
        protected Cliente() { }
        public Cliente(string nome, string email, string endereco)
        {
            Nome = nome;
            Email = email;
            Endereco = endereco;
            Status = StatusClienteEnum.Ativo;
            Locacoes = new List<Locacao>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public StatusClienteEnum Status { get; set; }
        public List<Locacao> Locacoes { get; set; }

        public string MudarStatusCliente(int statusId)
        {
            if (statusId == 0)
            {
                Status = StatusClienteEnum.Ativo;
            }
            else if (statusId == 1)
            {
                Status = StatusClienteEnum.Inativo;
            }
            else
            {
                throw new Exception("Status Inválido.");
            }
            
            return "Status alterado com sucesso!!!";
        }
    }
}
