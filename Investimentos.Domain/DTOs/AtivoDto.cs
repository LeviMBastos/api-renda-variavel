namespace Investimentos.Domain.DTOs
{
    public class AtivoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nome { get; set; } = null!;

        public CotacaoDto Cotacao { get; set; } = null!;
    }
}
