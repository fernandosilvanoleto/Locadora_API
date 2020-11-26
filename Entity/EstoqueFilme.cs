using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locadora_API.Entity
{
    public class EstoqueFilme
    {
        protected EstoqueFilme() { }
        public EstoqueFilme(int filmeId, int quantidade)
        {
            FilmeId = filmeId;
            Quantidade = quantidade;
        }
        public int Id { get; private set; }
        public int FilmeId { get; set; }
        public Filme Filme { get; set; }
        public int Quantidade { get; set; }

        public void DecrementarQuantidadeEstoque()
        {
            Quantidade = Quantidade - 1;
        }

        public void AumentarQuantidadeEstoque()
        {
            Quantidade = Quantidade + 1;
        }
    }
}
