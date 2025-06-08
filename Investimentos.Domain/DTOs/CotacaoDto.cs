namespace Investimentos.Domain.DTOs
{
    public class CotacaoDto
    {
        public int Id { get; set; }
        public decimal PrecoUnitario { get; set; }
        public DateTime DataHora { get; set; }
    }
}
