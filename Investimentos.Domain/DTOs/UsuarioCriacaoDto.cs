using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.DTOs
{
    public class UsuarioCriacaoDto
    {
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal PercentualCorretagem { get; set; }
    }
}
