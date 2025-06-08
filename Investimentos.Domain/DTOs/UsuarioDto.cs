namespace Investimentos.Domain.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal PercentualCorretagem { get; set; }

        public List<PosicaoDto>? Posicoes { get; set; }

        public List<OperacaoDto>? Operacoes { get; set; }
    }
}
