using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.DTOs
{
    public class CotacaoKafkaDto
    {
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public DateTime TradeTime { get; set; }
    }
}
