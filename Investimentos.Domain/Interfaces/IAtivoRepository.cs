using Investimentos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.Interfaces
{
    public interface IAtivoRepository
    {
        Task<Ativo?> BuscarPorCodigoAsync(string codigo);
        Task AdicionarAsync(Ativo ativo);
    }
}
