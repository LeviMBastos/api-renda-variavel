using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.DTOs
{
    public class UsuarioPlDto
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = null!;
        public decimal TotalPl { get; set; }
    }
}
