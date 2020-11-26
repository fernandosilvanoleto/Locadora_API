using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locadora_API.Enums
{
    public enum StatusLocacaoEnum
    {
        Alugado = 0,
        Devolvido = 1,
        DevolvidoComAtrasado = 2,
        Excluido = 3
    }
}
