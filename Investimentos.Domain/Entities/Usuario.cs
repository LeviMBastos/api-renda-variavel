namespace Investimentos.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal PercentualCorretagem { get; set; }

        public ICollection<Operacao> Operacoes { get; set; } = new List<Operacao>();
        public ICollection<Posicao> Posicoes { get; set; } = new List<Posicao>();
    }
}
