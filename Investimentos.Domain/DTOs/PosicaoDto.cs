namespace Investimentos.Domain.DTOs
{
    public class PosicaoDto
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal PL { get; set; }

        public AtivoDto Ativo { get; set; } = null!;
    }
}
