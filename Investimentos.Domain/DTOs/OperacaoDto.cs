namespace Investimentos.Domain.DTOs
{
    public class OperacaoDto
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string TipoOperacao { get; set; } = null!;
        public decimal Corretagem { get; set; }
        public DateTime DataHora { get; set; }

        public AtivoDto Ativo { get; set; } = null!;
    }
}
