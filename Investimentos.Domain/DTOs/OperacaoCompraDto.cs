using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.DTOs
{
    public class OperacaoCompraDto
    {
        public int UsuarioId { get; set; }
        public string CodigoAtivo { get; set; }
        public int Quantidade { get; set; }
    }
}
